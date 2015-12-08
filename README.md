# MTG Finance

## Server locations

There are two servers hosted on AWS that host information for the app: a
card info server and a card image server. The card info server serves
price information about cards in JSON format. The card image server
serves card images in JPEG format.

The card info server can be accessed using the following URL:

http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8000/mtg-card-prices/{card_name}/

Where {card_name} is the multiverse ID of the card whose price info
you want.

The JSON will be in the form {'cards': [{...}, {...}, ...]} where the 'cards' property holds
a list of JSON entries for each version of the requested card. Each dictionary in that list
will have the following fields:

- multiverse_id
- card_name - The official card name
- card_tcg_name - The name of the card on TCG Player
- card_display_name - The name of the card that should be displayed to the user
- card_image_name - The name of the image file for the card on the image server
- set_name - The official set name for the card
- set_tcg_name - The name of the set on TCG Player for the card
- set_display_name - The name of the set for the card that should be displayed to the user
- last_updated - A string in ISO datetime format that specifies the last
  time the pricing info for the card was updated.
- tcg_low - TCGPlayer low price (Will be 0 if no low price is available)
- tcg_mid - TCGPlayer mid price (Will be 0 if no mid price is available)
- tcg_avg_foil - TCGPlayer average foil price (Will be 0 if no foil price is available)
- tcg_link - URL for the card's page on TCGPlayer
- gatherer_link - URL for the card's page on Gatherer

If the card was not found in the price info database, a JSON response in the form
{'error': '...'} will be returned instead.


The card image server can be accessed using the following URL:

http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8080/mtg-card-images/{card_image_name}

Again, where {card_image_name} is the card_image_name field from the JSON given by the
price server for the card.


## Flow Using Server Data

This is what the flow should look like when a user attempts to access price info for a card:

1. After a user selects a card name for the search, request pricing info for the different versions of the card
by sending a request to the price server in the form mentioned above. IMPORTANT: Make sure the EXACT card name
from the MtgJSON is used. This means don't replace and special characters like 'Æ' in the card name for the name
used in the reqest URL.

2. The first request will return JSON in the form specified above. This JSON can be used to list out the different
versions of the card to user. The 'set_display_name' property of the JSON should be used as the display name for each
version of the card. When a user selects which version of the card they want, the image for that version can be
requested using the 'card_image_name' property of the JSON for that version in the form mentioned above.

3. After getting the image for the card version, the price info page for the version of the card selected by the
user can be displayed. On this page, use the 'card_display_name' property of the JSON for the selected card
version as the display name for the card. The JSON for the selected card includes 'tcg_low', 'tcg_mid', and
'tcg_avg_foil' properties for the pricing information for the card as well as 'tcg_link' and 'gatherer_link'
properties for external links for the card.
