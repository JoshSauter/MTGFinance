# MTG Finance

## Server locations

There are two servers hosted on AWS that host information for the app: a
card info server and a card image server. The card info server serves
price information about cards in JSON format. The card image server
serves card images in JPEG format.

The card info server can be accessed using the following URL:

http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8000/mtg-card-prices/{multiverse_id}/

Where {multiverse_id} is the multiverse ID of the card whose price info
you want.

The JSON will have the following fields:

- multiverse_id
- card_name
- set_name
- last_updated - A string in ISO datetime format that specifies the last
  time the pricing info for the card was updated.
- tcg_low - TCGPlayer low price (Will be 0 if no low price is available)
- tcg_mid - TCGPlayer mid price (Will be 0 if no mid price is available)
- tcg_avg_foil - TCGPlayer average foil price (Will be 0 if no foil price is available)
- tcg_link - URL for the card's page on TCGPlayer
- gatherer_link - URL for the card's page on Gatherer


The card image server can be accessed using the following URL:

http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8080/mtg-card-images/{multiverse_id}.jpg

Again, where {multiverse_id} is the multiverse ID of the card whose
image you want.
