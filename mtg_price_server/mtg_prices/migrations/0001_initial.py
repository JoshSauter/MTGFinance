# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
    ]

    operations = [
        migrations.CreateModel(
            name='MTGCardPrice',
            fields=[
                ('multiverse_id', models.IntegerField(serialize=False, primary_key=True)),
                ('card_name', models.CharField(max_length=175)),
                ('set_name', models.CharField(max_length=100)),
                ('last_updated', models.DateTimeField()),
                ('tcg_low', models.DecimalField(max_digits=10, decimal_places=2, null=True)),
                ('tcg_mid', models.DecimalField(max_digits=10, decimal_places=2, null=True)),
                ('tcg_avg_foil', models.DecimalField(max_digits=10, decimal_places=2, null=True)),
            ],
        ),
    ]
