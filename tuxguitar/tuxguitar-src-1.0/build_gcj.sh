#!/bin/bash
##SCRIPT DIR
DIR_NAME=`dirname "$0"`
DIR_NAME=`cd "$DIR_NAME"; pwd`

cd ${DIR_NAME}

export SWT_PATH=${DIR_NAME}/lib/swt.jar
export PREFIX=./
export TG_PREFIX=${DIR_NAME}/tuxguitar-1.0-linux-x86-gcj/
export TG_SOURCE_PATH=${DIR_NAME}/TuxGuitar/src/
export GCJFLAGS="-fsource=1.4 -fPIC"

make -C TuxGuitar/
make -C TuxGuitar/ install DESTDIR=${TG_PREFIX}

make -C TuxGuitar-alsa/
make -C TuxGuitar-alsa/ install

make -C TuxGuitar-compat/
make -C TuxGuitar-compat/ install

make -C TuxGuitar-gtp/
make -C TuxGuitar-gtp/ install

make -C TuxGuitar-ptb/
make -C TuxGuitar-ptb/ install

make -C TuxGuitar-tef/
make -C TuxGuitar-tef/ install

make -C TuxGuitar-midi/
make -C TuxGuitar-midi/ install

make -C TuxGuitar-lilypond/
make -C TuxGuitar-lilypond/ install

make -C TuxGuitar-musicxml/
make -C TuxGuitar-musicxml/ install

make -C TuxGuitar-ascii/
make -C TuxGuitar-ascii/ install

make -C TuxGuitar-converter/
make -C TuxGuitar-converter/ install
