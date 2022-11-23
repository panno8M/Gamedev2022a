# Gimmick - Portal

## 要点
* 離れた二点間を転送できる
* 転送開始時と転送完了時にアニメーション等任意の処理を挟める
* Unityのトリガーによって近くのポータルの情報を通知するが、転送処理とは完全に切り離されている
* 転送される側は任意の時点でそのポータルの転送先の情報を参照できる
* ポータル毎に転送の型を設定でき、転送される側は任意の時点でその情報を参照できる
* 転送の型は例えば、ドア、通路、ワームホールなど

## クラス図
![クラス図](../umls/gimmick_portal_class/Gimmick%20-%20Portal%20(class%20diagram).svg)

## シーケンス図
![シーケンス図 - 転送処理](../umls/gimmick_portal_sequence/Gimmick%20-%20Portal%20(sequence%20diagram)-page1.svg)
![シーケンス図 - 周辺ポータル通知処理](../umls/gimmick_portal_sequence/Gimmick%20-%20Portal%20(sequence%20diagram)-page2.svg)