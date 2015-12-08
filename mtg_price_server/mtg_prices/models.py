from django.db import models

GATHERER_LINK_FORMAT = (
    'http://gatherer.wizards.com/Pages/Card/Details.aspx?'
    'multiverseid={multiverse_id}')

class MTGCardPrice(models.Model):
    # ID for the card on Gatherer
    multiverse_id = models.IntegerField()

    # Official card name
    card_name = models.CharField(max_length=175)

    # Card name used on TCG Player
    card_tcg_name = models.CharField(max_length=175)

    # Name that should be displayed to user for the card
    card_display_name = models.CharField(max_length=175)

    # Name of the card's image on our price server
    card_image_name = models.CharField(max_length=50)

    # Official set name for the card
    set_name = models.CharField(max_length=100)

    # Set name used on TCG Player for the card
    set_tcg_name = models.CharField(max_length=100)

    # Name that should be displayed to user for the version of the card
    set_display_name = models.CharField(max_length=100)

    # Last time the card's pricing information was updated
    last_updated = models.DateTimeField()

    # TCG Player prices for the card
    tcg_low = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)
    tcg_mid = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)
    tcg_avg_foil = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)

    # Link to the card on TCG Player's website
    tcg_link = models.URLField(null=True)

    def get_json_dict(self):
        '''Returns a dictionary with the card's properties in JSON format.'''
        card_price_dict = {}
        card_price_dict['multiverse_id'] = self.multiverse_id
        card_price_dict['card_name'] = self.card_name
        card_price_dict['card_tcg_name'] = self.card_tcg_name
        card_price_dict['card_display_name'] = self.card_display_name
        card_price_dict['card_image_name'] = self.card_image_name

        card_price_dict['set_name'] = self.set_name
        card_price_dict['set_tcg_name'] = self.set_tcg_name
        card_price_dict['set_display_name'] = self.set_display_name
        card_price_dict['last_updated'] = self.last_updated.isoformat()

        card_price_dict['tcg_low'] = self._zero_if_none(self.tcg_low)
        card_price_dict['tcg_mid'] = self._zero_if_none(self.tcg_mid)
        card_price_dict['tcg_avg_foil'] = self._zero_if_none(self.tcg_avg_foil)

        card_price_dict['tcg_link'] = self.tcg_link
        card_price_dict['gatherer_link'] = GATHERER_LINK_FORMAT.format(
                multiverse_id=self.multiverse_id)

        return card_price_dict

    def _zero_if_none(self, item):
        return 0 if item is None else item

