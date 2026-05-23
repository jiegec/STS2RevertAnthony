#!/bin/sh
diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Cards $2/MegaCrit/Sts2/Core/Models/Cards > code-$3-$4-cards.diff
sed -i "s!$1!$3!g" code-$3-$4-cards.diff
sed -i "s!$2!$4!g" code-$3-$4-cards.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-$3-$4-cards.diff

diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Powers $2/MegaCrit/Sts2/Core/Models/powers > code-$3-$4-powers.diff
sed -i "s!$1!$3!g" code-$3-$4-powers.diff
sed -i "s!$2!$4!g" code-$3-$4-powers.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-$3-$4-powers.diff

diff -u10 -r -N $1/MegaCrit/Sts2/Core/Models/Monsters $2/MegaCrit/Sts2/Core/Models/monsters > code-$3-$4-monsters.diff
sed -i "s!$1!$3!g" code-$3-$4-monsters.diff
sed -i "s!$2!$4!g" code-$3-$4-monsters.diff
sed -E -i "s!\t[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}!!g" code-$3-$4-monsters.diff
