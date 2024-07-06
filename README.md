# 一些关于项目的详细介绍
使用Animator中混合树对人物是否锁敌、不同武器、奔跑、行走、下蹲、闪避翻滚、受击等动画进行切换过渡，并且使用AnimationLayer以及AvatarMask对人物移动时切换武器进行动画组合处理<br/>
在动画文件绘制Curves并在读取相应数据来将inplace动画模拟RootMotion动画来处理攻击、闪避时出现的位移<br/>
使用ScriptableObject对不同武器的攻击音效利用单例模式进行配置并读取<br/>
通过对鼠标按下时长的判定来确定人物是进行普通攻击还是蓄力攻击；按下鼠标中键，自动判断视野内是否存在可攻击敌人，若存在则对敌人进行视角的锁定<br/>
编写敌人AI脚本，实现敌人对玩家的索敌、攻击等操作，并使用有限状态机对敌人的攻击、待机、锁定敌人等状态进行管理和操作<br/>

## 【锁敌视角】<br/>
按下鼠标中键后，自动检测范围内是否存在敌人，若存在则将视角进行锁定，始终保持敌人在视角的正中央
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E8%A7%86%E8%A7%92%E9%94%81%E5%AE%9A%2000_00_00-00_00_30.gif)

## 【人物移动】<br/>
对于锁敌视角以及自由视角下的人物移动也做了不同的处理<br/>
MoveBlendTree如图:<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/MoveBlendTree.png)<br/>
具体实现:<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E8%87%AA%E7%94%B1%E8%A7%86%E8%A7%92%E5%A5%94%E8%B7%91%2000_00_00-00_00_30.gif)<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E8%A7%86%E8%A7%92%E9%94%81%E5%AE%9A%E4%B8%8B%E7%9A%84%E5%A5%94%E8%B7%91%2000_00_00-00_00_30.gif)

## 【人物翻滚】<br/>
人物可以通过翻滚来躲避敌人的攻击，翻滚动画的移动是通过绘制动画文件中的Curves，并在脚本中读取来模拟Root Motion的移动
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/Dodge%2000_00_00-00_00_30.gif)<br/>

## 【普通武器攻击】<br/>
普通长剑对敌人的攻击，根据攻击方向的不同，敌人受击后偏向的方向细节亦有所不同
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/SwordAttack%2000_00_00-00_00_30.gif)<br/>

## 【普通武器下的对敌人攻击的格挡】<br/>
在玩家被攻击后按下鼠标右键依然可以进入格挡模式，并对敌人的攻击进行格挡<br/>且针对敌人攻击的不同方向，武器格挡后剑锋的偏向也有所不同
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/Sword%E6%A0%BC%E6%8C%A1%2000_00_00-00_00_30.gif)

## 【大剑攻击】<br/>
在手持大剑状态下对敌人进行攻击，敌人的受击动画与普通长剑的不相同，最后一段攻击可以将敌人打倒<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E8%A7%86%E8%A7%92%E9%94%81%E5%AE%9A%E4%B8%8B%E7%9A%84%E5%A4%A7%E5%89%91%E6%94%BB%E5%87%BB%2000_00_00-00_00_30.gif)

## 【手持大剑状态下的受击】<br/>
当前为锁定视角的状态下手持大剑被敌人攻击的状态<br/>配合在动画文件中绘制的Curves，实现被攻击后的向后移动
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E5%A4%A7%E5%89%91%E5%8F%97%E5%87%BB%2000_00_00-00_00_30.gif)

## 【手持大剑状态下的格挡&&敌人的冲刺攻击】<br/>
手持大剑状态下同样可以进入格挡状态对敌人的攻击进行格挡；<br/>
同时从该动画中可以看出敌人存在一个冲刺攻击，冲刺攻击的移动同样是通过绘制Curves并读取来实现的<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E5%A4%A7%E5%89%91%E6%A0%BC%E6%8C%A1%2000_00_00-00_00_30.gif)

## 【移动状态下切换武器】<br/>
由于所给资源中并没有移动切换武器的动画，故使用AnimationLayer以及AvatarMask对人物移动时切换武器进行动画组合处理<br/>
将切换武器的动画单独作为一个层级，并新建一个Avatar选取上半身的IK点，放入Mask中，Blending设置为Override<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/ba730883-533e-4d2c-b20c-b86cc2228057.png)<br/>
ChangeWeapom动画层:<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/05bfc4bb-e0dd-475a-89b2-85eff8d7d7ab.png)<br/>
最终实现:<br/>
![image](https://github.com/GWX4899/RPG/blob/main/RPGGif/%E7%A7%BB%E5%8A%A8%E7%8A%B6%E6%80%81%E4%B8%8B%E5%88%87%E6%8D%A2%E6%AD%A6%E5%99%A8%2000_00_00-00_00_30.gif)


