from django.conf import settings
from django.core.management.base import BaseCommand, CommandError
from django.utils import timezone
from mtg_prices.models import MTGCardPrice

from xml.etree import ElementTree
import os
import requests
import shutil

from nameparser import config, HumanName
config.CONSTANTS.prefixes.remove('van')

MTG_JSON_URL = 'http://mtgjson.com/json/AllSets.json'

GATHERER_IMAGE_URL_FORMAT = (
        'http://gatherer.wizards.com/Handlers/Image.ashx'
        '?multiverseid={multiverse_id}&type=card')

GATHERER_FLIPPED_IMAGE_URL_FORMAT = (
        'http://gatherer.wizards.com/Handlers/Image.ashx'
        '?multiverseid={multiverse_id}&type=card&options=rotate180')

TCG_PLAYER_CARD_URL_FORMAT = (
        'http://partner.tcgplayer.com/x3/phl.asmx/p?pk=MTGFINANCE'
        '&s={set_name}&p={card_name}')

CARD_IMAGE_DIRECTORY = 'mtg-card-images/'

FLIPPED_IMAGE_SUFFIX = 'flipped'
CARD_IMAGE_EXTENSION = '.jpg'

TCG_TRANS_TABLE = {
    u'Æ': u'Ae',
    u'ö': u'o',
    u'û': u'u',
    u'á': u'a',
    u'é': u'e',
    u'&': u'%26',
}
TCG_SPLIT_CARD_SEP = ' // '
DISPLAY_SPLIT_CARD_SEP = ' // '
REVERSABLE_CARD_LAYOUTS = ['flip', 'double-faced']

INCOMPLETE_SETS_ON_TCG_PLAYER = [
    'Vanguard',
]

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
    'Dragon Con': 'Media Promos',
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
    'Media Inserts': 'Media Promos',
    'Modern Event Deck 2014': 'Magic Modern Event Deck',
    'Ravnica: City of Guilds': 'Ravnica',
    'Time Spiral "Timeshifted"': 'Timeshifted',
}

TCG_SET_EXCEPTIONS = {
    'Stairs to Infinity': 'Release Event Cards',
}

TCG_SPECIAL_CARD_NAME = {
    10490: 'Kongming, “Sleeping Dragon”',
    10713: 'Pang Tong, “Young Phoenix”',
    74237: 'Our Market Research...',
    73935: 'Ach! Hans, Run!',
    9780: 'B.F.M. (Big Furry Monster Left)',
    9844: 'B.F.M. (Big Furry Monster Right)',
    9757: 'The Ultimate Nightmare of Wizards of the Coast® Cu',
    949: 'Naf\'s Asp',
    950: 'Naf\'s Asp (Light)',
    394407: 'Soldier Token',
    394383: 'Kraken Token',
    190199: 'Elemental Shaman Token',
    386322: 'Goblin Token',
    209163: 'Hornet Token',
    401721: 'Hellion Token',
    401722: 'Plant Token',
    401718: 'Eldrazi Spawn Token (Briclot)',
    401719: 'Eldrazi Spawn Token (Meignaud)',
    401720: 'Eldrazi Spawn Token (Mark Tedin)',
    4979: 'Pegasus Token',
    5472: 'Soldier Token',
    5601: 'Zombie Token',
    5503: 'Goblin Token',
    5560: 'Sheep Token',
    5607: 'Squirrel Token',
    74358: 'Who/What/When/Where/Why',
}

TCG_MULTIART_DISPLAY_SUFFIX = {
    78968: ' (160a Sword)', # Brothers Yamazaki
    85106: ' (160b Pike)', # Brothers Yamazaki
    1938: ' (Map)', # Brassclaw Orcs
    1940: ' (Claws)', # Brassclaw Orcs
    1937: ' (Pike)', # Brassclaw Orcs
    1966: ' (Horned Helm)', # Brassclaw Orcs
    1850: ' (Wolf)', # Hymn to Tourach
    1851: ' (Cloak)', # Hymn to Tourach
    1849: ' (Circle)', # Hymn to Tourach
    1852: ' (Table)', # Hymn to Tourach
    1072: ' (Spring)', # Mishra's Factory
    1071: ' (Fall)', # Mishra's Factory
    1074: ' (Winter)', # Mishra's Factory
    1073: ' (Summer)', # Mishra's Factory
    1077: ' (Uneven Horizon)', # Strip Mine
    1076: ' (Even Horizon)', # Strip Mine
    1078: ' (No Horizon)', # Strip Mine
    1079: ' (Tower)', # Strip Mine
    1083: ' (Tower)', # Urza's Mine
    1080: ' (Clawed Sphere)', # Urza's Mine
    1082: ' (Pulley)', # Urza's Mine
    1081: ' (Mouth)', # Urza's Mine
    1085: ' (Columns)', # Urza's Power Plant
    1084: ' (Bug)', # Urza's Power Plant
    1086: ' (Sphere)', # Urza's Power Plant
    1087: ' (Rock in Pot)', # Urza's Power Plant
    1088: ' (Forest)', # Urza's Tower
    1091: ' (Shore)', # Urza's Tower
    1090: ' (Plains)', # Urza's Tower
    1089: ' (Mountains)', # Urza's Tower
    3040: ' (Clouds)', # Aesthir Glider
    3041: ' (Moon)', # Aesthir Glider
    3098: ' (Sword)', # Arcane Denail
    3097: ' (Axe)', # Arcane Denail
    3130: ' (Red Robe)', # Deadly Insect
    3129: ' (Bird)', # Deadly Insect
    3133: ' (Female)', # Elvish Ranger
    3132: ' (Male)', # Elvish Ranger
    3105: ' (Underwater)', # False Demise
    3106: ' (Cave-in)', # False Demise
    3073: ' (Knife)', # Feast or Famine
    3074: ' (Falling into Pit)', # Feast or Famine
    3142: ' (4 Gorillas)', # Gorilla Chieftain
    3143: ' (2 Gorillas)', # Gorilla Chieftain
    3175: ' (Cliff)', # Guerrilla Tactics
    3176: ' (Kneeling Knight)', # Guerrilla Tactics
    3202: ' (Bear)', # Kjeldoran Pride
    3201: ' (Eagle)', # Kjeldoran Pride
    3111: ' (Book)', # Lat-Nam's Legacy
    3110: ' (Scroll)', # Lat-Nam's Legacy
    3083: ' (Red Armor)', # Lim-Dûl's High Guard
    3086: ' (Close-up)', # Phantasmal Fiend
    3085: ' (Doorway)', # Phantasmal Fiend
    3208: ' (Line-up)', # Reinforcements
    3207: ' (Orc)', # Reinforcements
    3209: ' (Red Dragon)', # Reprisal
    3210: ' (Green Monster)', # Reprisal
    3116: ' (Old Woman)', # Soldevi Sage
    3117: ' (2 Candles)', # Soldevi Sage
    3120: ' (Flying Left)', # Storm Crow
    3119: ' (Flying Right)', # Storm Crow
    3183: ' (Female)', # Storm Shaman
    3184: ' (Male)', # Storm Shaman
    3096: ' (Fallen Tree)', # Swamp Mosquito
    3095: ' (Brown Trees)', # Swamp Mosquito
    2889: ' (Mouth)', # Urza's Mine
    2890: ' (Pulley)', # Urza's Mine
    2891: ' (Tower)', # Urza's Mine
    2888: ' (Clawed Sphere)', # Urza's Mine
    2895: ' (Rock in Pot)', # Urza's Power Plant
    2894: ' (Sphere)', # Urza's Power Plant
    2892: ' (Bug)', # Urza's Power Plant
    2893: ' (Columns)', # Urza's Power Plant
    2897: ' (Mountains)', # Urza's Tower
    2898: ' (Plains)', # Urza's Tower
    2899: ' (Shore)', # Urza's Tower
    2896: ' (Forest)', # Urza's Tower
    2970: ' [Version 2]', # Hungry Mist
    2969: ' [Version 1]', # Hungry Mist
    3022: ' [Version 2]', # Mesa Falcon
    3021: ' [Version 1]', # Mesa Falcon
    393898: ' (Garruk vs Liliana)', # Giant Growth
    393959: ' (Goblins vs Elves)', # Giant Growth
    393883: ' (Divine v. Demonic)', # Corrupt
    394018: ' (Garruk v. Liliana)', # Corrupt
    393899: ' (Garruk vs Liliana)', # Harmonize
    393966: ' (Goblins vs Elves)', # Harmonize
}


class MtgNames:
    def __init__(self, official_name, tcg_name='', display_name='', suffix=''):
        self.official_name = official_name
        self.tcg_name = tcg_name
        self.display_name = display_name
        self.suffix = suffix

    def __repr__(self):
        return u'Official: {0}\nTCG: {1}\nDisplay: {2}\nSuffix: {3}'.format(
                self.official_name, self.tcg_name, self.display_name,
                self.suffix)


class Command(BaseCommand):
    help = 'Updates the card price data in the database'

    def handle(self, *args, **options):
        r = requests.get(MTG_JSON_URL)
        mtg_json = r.json()

        for set_code, set_json in mtg_json.items():
            if not self._can_track_set(set_json):
                continue

            for card_json in set_json['cards']:
                multiverse_id = card_json.get('multiverseid')
                if multiverse_id is None:
                    continue

                set_names = self._get_set_names(card_json, set_json)
                card_names, flipped = self._get_card_names(
                        card_json, set_names.tcg_name)
                set_names.display_name = set_names.tcg_name + card_names.suffix

                r = requests.get(TCG_PLAYER_CARD_URL_FORMAT.format(
                        set_name=set_names.tcg_name,
                        card_name=card_names.tcg_name))
                if r.status_code != 200:
                    print('\n{}'.format(r.status_code))
                    print(card_names)
                    print(set_names)
                    continue

                price_xml = ElementTree.fromstring(r.content)
                successful = self._update_card_price(
                        card_names, set_names, flipped, multiverse_id, price_xml)
                if not successful:
                    print('\nFAIL'.format(r.status_code))
                    print(card_names)
                    print(set_names)
                    continue
                else:
                    self._store_card_image(multiverse_id, flipped)
                    print('Success\t{0}\t{1}'.format(
                            card_names.tcg_name, set_names.tcg_name))


    def _can_track_set(self, set_json):
        if set_json.get('onlineOnly', False):
            return False
        if set_json['name'] in INCOMPLETE_SETS_ON_TCG_PLAYER:
            return False

        return True

    def _get_card_names(self, card_json, set_tcg_name):
        card_names = MtgNames(card_json['name'])
        card_names.tcg_name = card_names.official_name
        multiverse_id = card_json.get('multiverseid')
        flipped = False

        if card_json['layout'] == 'split':
            card_names.tcg_name = TCG_SPLIT_CARD_SEP.join(card_json['names'])
            card_names.display_name = DISPLAY_SPLIT_CARD_SEP.join(card_json['names'])

        card_base_tcg_name = TCG_SPECIAL_CARD_NAME.get(
                multiverse_id, card_names.tcg_name)
        card_names.tcg_name = card_base_tcg_name
        card_names.suffix = self._get_version_suffix(
                card_json, card_names.tcg_name, set_tcg_name)
        card_names.tcg_name = card_names.tcg_name + card_names.suffix
        card_names.tcg_name = self._translate_to_tcg(
                card_names.tcg_name, set_tcg_name)
        if not card_names.display_name:
            card_names.display_name = card_base_tcg_name + card_names.suffix

        if card_json['layout'] in REVERSABLE_CARD_LAYOUTS and \
                not self._test_tcg_request(card_names.tcg_name, set_tcg_name):
            card_names.tcg_name = self._get_reverse_name(card_json)
            card_names.tcg_name = self._translate_to_tcg(
                    card_names.tcg_name, set_tcg_name)
            flipped = True if card_json['layout'] == 'flip' else flipped

        return (card_names, flipped)

    def _get_set_names(self, card_json, set_json):
        set_names = MtgNames(set_json['name'])
        set_names.tcg_name = TCG_SET_NAME.get(
                set_names.official_name, set_names.official_name)
        set_names.tcg_name = TCG_SET_EXCEPTIONS.get(
                card_json['name'], set_names.tcg_name)
        return set_names

    def _get_version_suffix(self, card_json, card_tcg_name, set_tcg_name):
        supertypes = card_json.get('supertypes', [])
        if not bool(card_json.get('variations')) and 'Basic' not in supertypes:
            return ''

        suffix = TCG_MULTIART_DISPLAY_SUFFIX.get(card_json['multiverseid'])
        if suffix and self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        artist_name = HumanName(card_json['artist'])
        if artist_name.suffix:
            artist_last_name = artist_name.last + ' ' + artist_name.suffix
        else:
            artist_last_name = artist_name.last
        suffix = ' ({0})'.format(artist_last_name)
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' [Version 2]'
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' ({0})'.format(card_json.get('number'))
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' - {0}'.format(set_tcg_name)
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' - Full Art'
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' ({0}) - Full Art'.format(card_json.get('number'))
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        suffix = ' ({0} Full Art)'.format(card_json.get('number'))
        if self._test_tcg_request(card_tcg_name + suffix, set_tcg_name):
            return suffix

        return ''

    def _translate_to_tcg(self, card_name, set_tcg_name):
        trans_card_name = card_name
        for find_str, replace_str in TCG_TRANS_TABLE.items():
            trans_card_name = trans_card_name.replace(find_str, replace_str)

        if (trans_card_name != card_name and
                not self._test_tcg_request(trans_card_name, set_tcg_name)):
            return card_name
        return trans_card_name

    def _get_reverse_name(self, card_json):
        return next(n for n in card_json['names'] if n != card_json['name'])

    def _test_tcg_request(self, card_name, set_name):
        r = requests.get(TCG_PLAYER_CARD_URL_FORMAT.format(
                set_name=set_name, card_name=card_name))
        return r.status_code == 200


    def _update_card_price(self, card_names, set_names, flipped,
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

        card_price_obj = MTGCardPrice.objects.filter(
                card_name=card_names.official_name,
                card_tcg_name=card_names.tcg_name,
                set_name=set_names.official_name)
        if card_price_obj.exists():
            card_price_obj.update(
                last_updated=timezone.now(), tcg_low=tcg_low, tcg_mid=tcg_mid,
                tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)

        else:
            card_image_name = str(multiverse_id)
            if flipped:
                card_image_name += FLIPPED_IMAGE_SUFFIX
            card_image_name += CARD_IMAGE_EXTENSION

            card_price_obj = MTGCardPrice(
                    multiverse_id=multiverse_id,
                    card_name=card_names.official_name,
                    card_tcg_name=card_names.tcg_name,
                    card_display_name=card_names.display_name,
                    card_image_name=card_image_name,
                    set_name=set_names.official_name,
                    set_tcg_name=set_names.tcg_name,
                    set_display_name=set_names.display_name,
                    last_updated=timezone.now(),
                    tcg_low=tcg_low, tcg_mid=tcg_mid,
                    tcg_avg_foil=tcg_avg_foil, tcg_link=tcg_link)
            card_price_obj.save()
        return True


    def _store_card_image(self, multiverse_id, flipped):
        suffix = FLIPPED_IMAGE_SUFFIX if flipped else ''
        image_dir = os.path.join(settings.MEDIA_ROOT, CARD_IMAGE_DIRECTORY);
        image_path = '{0}{1}{2}{3}'.format(
                image_dir, multiverse_id, suffix, CARD_IMAGE_EXTENSION)
        if os.path.isfile(image_path):
            return

        if flipped:
            image_url = GATHERER_FLIPPED_IMAGE_URL_FORMAT.format(
                    multiverse_id=multiverse_id)
        else:
            image_url = GATHERER_IMAGE_URL_FORMAT.format(
                    multiverse_id=multiverse_id)
        r = requests.get(image_url, stream=True)
        with open(image_path, 'wb') as out_file:
            shutil.copyfileobj(r.raw, out_file)
        del r

