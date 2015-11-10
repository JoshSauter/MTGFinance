APPNAME=mtg_price_server
APPDIR=/home/ubuntu/$APPNAME/

LOGFILE=$APPDIR'gunicorn.log'
ERRORFILE=$APPDIR'gunicorn-error.log'

NUM_WORKERS=3

ADDRESS=0.0.0.0:8000

cd $APPDIR

source ~/.bashrc
workon mtg_finance

exec gunicorn $APPNAME.wsgi:application \
  -w $NUM_WORKERS --bind=$ADDRESS \
  --log-level=debug \
  --log-file=$LOGFILE 2>>$LOGFILE 1>>$ERRORFILE &
