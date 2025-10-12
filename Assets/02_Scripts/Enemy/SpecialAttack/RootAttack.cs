using UnityEngine;

[CreateAssetMenu(fileName = "SA_Root", menuName = "Enemy/Special Attacks/Root")]
public class RootAttack : SpecialAttackBase
{
    [Header("----- 속박 설정 -----")]
    [SerializeField] float _duration = 3f;   //속박 지속 시간

    public override void Execute(Transform attacker, Transform target)
    {
        //Player 스크립트에서 처리

        Debug.Log($"특수 공격 : 속박. 대상 : {target}. {_duration}초 간 이동 불가 상태로 만듭니다!");
    }
}
