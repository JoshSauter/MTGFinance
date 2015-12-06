from django.http import JsonResponse
from mtg_prices.models import MTGCardPrice

def get_card_prices_json(request, card_name):
    card_price_objs = MTGCardPrice.objects.filter(card_name=card_name)
    if not card_price_objs.exists():
        return JsonResponse({'error': 'Card does not exist in database'})

    json_dict = {'cards': [c.get_json_dict() for c in card_price_objs]}
    return JsonResponse(json_dict)

