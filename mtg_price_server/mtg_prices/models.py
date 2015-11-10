from django.db import models

GATHERER_LINK_FORMAT = (
    'http://gatherer.wizards.com/Pages/Card/Details.aspx?'
    'multiverseid={multiverse_id}')

class MTGCardPrice(models.Model):
    multiverse_id = models.IntegerField(primary_key=True)
    card_name = models.CharField(max_length=175)
    set_name = models.CharField(max_length=100)
    last_updated = models.DateTimeField()

    tcg_low = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)
    tcg_mid = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)
    tcg_avg_foil = models.DecimalField(
            max_digits=10, decimal_places=2, null=True)
    tcg_link = models.URLField(null=True)

    def get_json_dict(self):
        card_price_dict = {}
        card_price_dict['multiverse_id'] = self.multiverse_id
        card_price_dict['card_name'] = self.card_name
        card_price_dict['set_name'] = self.set_name
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
