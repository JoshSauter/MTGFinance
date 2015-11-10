from django.http import JsonResponse
from mtg_prices.models import MTGCardPrice

def get_card_price_json(request, multiverse_id):
    card_price_obj = MTGCardPrice.objects.get(multiverse_id=multiverse_id);
    return JsonResponse(card_price_obj.get_json_dict())

