from django.http import JsonResponse
from mtg_prices.models import MTGCardPrice

def get_card_price_json(request, multiverse_id):
    card_price_obj = MTGCardPrice.objects.filter(multiverse_id=multiverse_id)
    if not card_price_obj.exists():
        return JsonResponse({'error': 'Card does not exist in database'})
    return JsonResponse(card_price_obj[0].get_json_dict())

