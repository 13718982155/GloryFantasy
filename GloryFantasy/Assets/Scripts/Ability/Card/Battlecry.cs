using GamePlay;
using IMessage;


using Ability.Buff;
namespace Ability
{
    public class Battlecry : Ability
    {
        private Trigger _trigger;
        private GameUnit.GameUnit _unit;

        public override void Init(string abilityId)
        {
            base.Init(abilityId);
            
            _unit = GetComponent<GameUnit.GameUnit>();
            gameObject.AddComponent<BattlecryBuff>();
            //_trigger = new DelayedTrigger(
            //    this.GetCardReceiver(this),
            //    0,
            //    (int)MessageType.MPEnd,
            //    () => { _unit.atk -= 2; }
            //    );
            //MsgDispatcher.RegisterMsg(_trigger, abilityId + "--DT", true);
            //_unit.atk += 2;
        }
        
        
    }

    //Ҫ�õ�Buff
    public class BattlecryBuff : Buff.Buff
    {
        //�趨Buff�ĳ�ʼ��
        protected override void InitialBuff()
        {
            //�趨Buff���������ڣ�����д��,����ʹ�õڶ��֣��Ƚ�ֱ��
            SetLife(2f);

            //BuffҪ�������飬������Abilityһ��ҲдTrigger��Ҳ����ֻ����һЩ��ֵ��������Abilityһ������һ�׹��ߺ�����
            GameUnit.GameUnit unit = GetComponent<GameUnit.GameUnit>();
            unit.atk += 2;
        }

        //�趨Buff CD����ʱ�Ĳ���
        public override void OnSubtractBuffLife()
        {
            //���¿���
        }

        //�趨Buff��ʧʱ�������
        protected override void OnDisappear()
        {
            GetComponent<GameUnit.GameUnit>().atk -= 2;
        }
    }
}