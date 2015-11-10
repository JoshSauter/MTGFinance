from django.conf import settings
from django.core.management.base import BaseCommand, CommandError
from django.utils import timezone

from mtg_prices.models import MTGCardPrice
from xml.etree import ElementTree
import os
import requests
import shutil

MTG_JSON_URL = 'http://mtgjson.com/json/AllSets.json'

GATHERER_IMAGE_URL_FORMAT = (
        'http://gatherer.wizards.com/Handlers/Image.ashx'
        '?multiverseid={multiverse_id}&type=card')

TCG_PLAYER_CARD_URL_FORMAT = (
        'http://partner.tcgplayer.com/x3/phl.asmx/p?pk=MTGFINANCE'
        '&s={set_name}&p={card_name}')

CARD_IMAGE_DIRECTORY = 'mtg-card-images/'

class Command(BaseCommand):
    help = 'Updates the card price data in the database'

    def handle(self, *args, **options):
        r = requests.get(MTG_JSON_URL)
        mtg_json = r.json()

        can_break = False
        for set_code, set_json in mtg_json.items():
            if set_json.get('onlineOnly', False):
                continue

            set_name = set_json['name']
            for card_json in set_json['cards']:
                card_name = card_json['name']
                multiverse_id = card_json.get('multiverseid')
                if multiverse_id is None:
                    continue

                r = requests.get(TCG_PLAYER_CARD_URL_FORMAT.format(
                        set_name=set_name, card_name=card_name))
                if r.status_code != 200:
                    print(r.status_code)
                    if r.status_code == 500:
                        print('{}\t{}'.format(set_name, card_name))
                        break
                    continue

                price_xml = ElementTree.fromstring(r.content)
                self._update_card_price(card_name, set_name,
                                        multiverse_id, price_xml)
                self._get_card_image(multiverse_id)
                can_break = True
                break
            if can_break:
                break

    def _update_card_price(self, card_name, set_name,
                           multiverse_id, price_xml):
        tcg_low = float(price_xml.find('.//lowprice').text)
        tcg_mid = float(price_xml.find('.//avgprice').text)
        tcg_avg_foil = float(price_xml.find('.//foilavgprice').text)
        tcg_link = price_xml.find('.//link').text

        card_price_obj = MTGCardPrice.objects.filter(multiverse_id=multiverse_id)
        if card_price_obj.exists():
            card_price_obj[0].update(
                last_updated=timezone.now(), tcg_low=tcg_low, tcg_mid=tcg_mid,
                tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)

        else:
            card_price_obj = MTGCardPrice(
                    multiverse_id=multiverse_id, card_name=card_name,
                    set_name=set_name, last_updated=timezone.now(),
                    tcg_low=tcg_low, tcg_mid=tcg_mid,
                    tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)
            card_price_obj.save()

    def _get_card_image(self, multiverse_id):
        image_path = os.path.join(settings.MEDIA_ROOT, CARD_IMAGE_DIRECTORY);
        if os.path.isfile('{0}{1}.jpg'.format(image_path, multiverse_id)):
            return

        r = requests.get(
                GATHERER_IMAGE_URL_FORMAT.format(multiverse_id=multiverse_id),
                stream=True)
        with open('{0}{1}.jpg'.format(image_path, multiverse_id), 'wb') as out_file:
            shutil.copyfileobj(r.raw, out_file)
        del r

