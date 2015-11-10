# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('mtg_prices', '0001_initial'),
    ]

    operations = [
        migrations.AddField(
            model_name='mtgcardprice',
            name='tcg_link',
            field=models.URLField(null=True),
        ),
    ]
