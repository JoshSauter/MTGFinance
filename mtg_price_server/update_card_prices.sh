#!/bin/bash
date_str=`date "+%H:%M:%S_%m-%d-%Y_%:z"`

cd `dirname $0`
source ~/.bashrc
workon mtg_finance

python3 manage.py update_card_prices > update_logs/$date_str.log
