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

TCG_SET_NAME = {
    'Seventh Edition': '7th Edition',
    'Eighth Edition': '8th Edition',
    'Ninth Edition': '9th Edition',
    'Tenth Edition': '10th Edition',
    'Limited Edition Alpha': 'Alpha Edition',
    'Limited Edition Beta': 'Beta Edition',
    'Modern Masters 2015 Edition': 'Modern Masters 2015',
    'European Land Program': 'European Lands',
    'Gateway': 'Gateway Promos',
    'From the Vault: Annihilation (2014)': 'From the Vault: Annihilation',
    'Planechase 2012 Edition': 'Planechase 2012',
    'Arena League': 'Arena Promos',
    'Asia Pacific Land Program': 'APAC Lands',
    'Champs and States': 'Champs Promos',
    'Coldsnap Theme Decks': 'Coldsnap Theme Deck Reprints',
    'Magic: The Gathering-Commander': 'Commander',
    'Commander 2013 Edition': 'Commander 2013',
    'Magic: The Gathering—Conspiracy': 'Conspiracy',
    'Deckmasters': 'Deckmasters Garfield vs Finkel',
    'Duel Decks Anthology, Divine vs. Demonic': 'Duel Decks: Anthology',
    'Duel Decks Anthology, Elves vs. Goblins': 'Duel Decks: Anthology',
    'Duel Decks Anthology, Garruk vs. Liliana': 'Duel Decks: Anthology',
    'Duel Decks Anthology, Jace vs. Chandra': 'Duel Decks: Anthology',
    'Friday Night Magic': 'FNM Promos',
    'Grand Prix': 'Grand Prix Promos',
    'Guru': 'Guru Lands',
    'Magic Game Day': 'Game Day Promos',
    'International Collector\'s Edition': 'International Edition',
    'Magic 2010': 'Magic 2010 (M10)',
    'Magic 2011': 'Magic 2011 (M11)',
    'Magic 2012': 'Magic 2012 (M12)',
    'Magic 2013': 'Magic 2013 (M13)',
    'Magic 2014 Core Set': 'Magic 2014 (M14)',
    'Magic 2015 Core Set': 'Magic 2015 (M15)',
    'Modern Event Deck 2014': 'Magic Modern Event Deck',
    'Ravnica: City of Guilds': 'Ravnica',
    'Time Spiral "Timeshifted"': 'Timeshifted',
}

class Command(BaseCommand):
    help = 'Updates the card price data in the database'

    def handle(self, *args, **options):
        r = requests.get(MTG_JSON_URL)
        mtg_json = r.json()
        current_ids = MTGCardPrice.objects.values('multiverse_id').distinct()
        current_ids = [row['multiverse_id'] for row in current_ids]

        for set_code, set_json in mtg_json.items():
            if set_json.get('onlineOnly', False):
                continue

            set_name = self._get_set_name(set_json)
            for card_json in set_json['cards']:
                card_name = self._get_card_name(card_json)
                multiverse_id = card_json.get('multiverseid')
                if multiverse_id is None:
                    continue
                if multiverse_id in current_ids:
                    print('Skipping {}\t\t{}'.format(set_name, card_name))
                    continue

                r = requests.get(TCG_PLAYER_CARD_URL_FORMAT.format(
                        set_name=set_name, card_name=card_name))
                if r.status_code != 200:
                    print('{}\t{}\t\t{}'.format(
                        r.status_code, set_name, card_name))
                    continue

                price_xml = ElementTree.fromstring(r.content)
                successful = self._update_card_price(card_name, set_name,
                                                     multiverse_id, price_xml)
                if not successful:
                    print('FAIL\t{}\t\t{}'.format(set_name, card_name))
                    continue
                else:
                    self._get_card_image(multiverse_id)
                    print('{}\t\t{}'.format(set_name, card_name))


    def _get_set_name(self, set_json):
        return TCG_SET_NAME.get(set_json['name'], set_json['name'])


    def _get_card_name(self, card_json):
        return card_json['name'].replace('Æ', 'Ae')


    def _update_card_price(self, card_name, set_name,
                           multiverse_id, price_xml):
        tcg_low = price_xml.find('.//lowprice')
        tcg_mid = price_xml.find('.//avgprice')
        tcg_avg_foil = price_xml.find('.//foilavgprice')
        tcg_link = price_xml.find('.//link')

        if tcg_low is None or tcg_mid is None or tcg_avg_foil is None or \
                tcg_link is None:
            return False
        else:
            tcg_low = float(tcg_low.text)
            tcg_mid = float(tcg_mid.text)
            tcg_avg_foil = float(tcg_avg_foil.text)
            tcg_link = tcg_link.text

        card_price_obj = MTGCardPrice.objects.filter(multiverse_id=multiverse_id)
        if card_price_obj.exists():
            card_price_obj.update(
                last_updated=timezone.now(), tcg_low=tcg_low, tcg_mid=tcg_mid,
                tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)

        else:
            card_price_obj = MTGCardPrice(
                    multiverse_id=multiverse_id, card_name=card_name,
                    set_name=set_name, last_updated=timezone.now(),
                    tcg_low=tcg_low, tcg_mid=tcg_mid,
                    tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)
            card_price_obj.save()
        return True


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

