# ADOFAI-EventLib
ADOFAI Event Library

## 어떤 목적으로 만들어진 라이브러리인가요?
LevelEventType과 LevelEventProperties를 직접 건드리지 않고도 새로운 이벤트를 추가할 수 있습니다.

## EventLib을 통해 새로 만들어진 이벤트가 EventLib이 없는 환경에서도 적용되나요?
__전적으로 모드 개발자에게 달려 있습니다.__<br>
EventLib은 기본적으로 커스텀 이벤트를 주석 형식으로 저장하기에, 커스텀 이벤트가 있는 레벨을 모드가 없는 환경에서 열 때 문제가 발생하지는 않습니다. 다만 커스텀 이벤트를 구현하는 과정에서 특정 모드에 의존하는 기능을 사용하였다면, 그 경우에는 레벨이 의도한 대로 작동하지 않을 수 있습니다. <br>
모드 의존적 기능과 비의존적 기능을 구분하기 위해, [RequiredMod] Attribute를 사용할 수 있습니다.

아래는 EventLib을 사용해 만든 모드 비의존적 커스텀 이벤트의 예시입니다. (실제로 보이는 부분은 커스텀 이벤트뿐입니다.)
<img width="50%" src="https://github.com/papertoy1127/ADOFAI-EventLib/assets/46876705/c8426d10-ce06-412c-b75d-11c87395c0c5"/>

## EventLib은 언제 출시되나요?
몰라요 시험기간이에요 바빠요

#### Enum Patch에 도움을 주신 [c3nb(C##)](https://github.com/c3nb)님 감사합니다!
