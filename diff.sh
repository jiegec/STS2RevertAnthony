#!/bin/sh
diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Cards $2/MegaCrit/Sts2/Core/Models/Cards > code-v0.99.1-v0.103.2-cards.diff
sed -i "s!$1!v0.99.1!g" code-v0.99.1-v0.103.2-cards.diff
sed -i "s!$2!v0.103.2!g" code-v0.99.1-v0.103.2-cards.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-v0.99.1-v0.103.2-cards.diff

diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Powers $2/MegaCrit/Sts2/Core/Models/powers > code-v0.99.1-v0.103.2-powers.diff
sed -i "s!$1!v0.99.1!g" code-v0.99.1-v0.103.2-powers.diff
sed -i "s!$2!v0.103.2!g" code-v0.99.1-v0.103.2-powers.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-v0.99.1-v0.103.2-powers.diff

diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Monsters $2/MegaCrit/Sts2/Core/Models/monsters > code-v0.99.1-v0.103.2-monsters.diff
sed -i "s!$1!v0.99.1!g" code-v0.99.1-v0.103.2-monsters.diff
sed -i "s!$2!v0.103.2!g" code-v0.99.1-v0.103.2-monsters.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-v0.99.1-v0.103.2-monsters.diff