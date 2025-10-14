using UnityEngine;

[CreateAssetMenu(fileName = "SA_Burn", menuName = "Enemy/Special Attacks/Burn")]
public class BurnAttack : SpecialAttackBase
{
    [Header("----- 발사체 설정 -----")]
    [SerializeField] Transform _firePos;
    [SerializeField] GameObject _projectilePrefab;
    [SerializeField] float _projectileSpeed = 15f;

    [Header("----- 화상 설정 -----")]
    [SerializeField] float _totalDamage = 50f;  //들어갈 총 데미지
    [SerializeField] float _duration = 5f;      //화상 지속 시간
    [SerializeField] float _tickInterval = 1f;  //화상 시간 간격

    public override void Execute(Transform attacker, Transform target)
    {
        //Debug.Log($"특수 공격 : 화상. 대상 : {target}. {_duration}초에 걸쳐 {_totalDamage}만큼의 피해를 입힙니다!");

        if (_projectilePrefab == null)
        {
            Debug.LogError("화염구 프리팹이 연결되지 않았습니다.");
            return;
        }

        //발사 위치를 적의 위치보다 약간 앞으로 설정
        Vector3 spawnPos = attacker.position + attacker.forward * 1.2f;
        //발사 방향은 타겟(플레이어)를 향해
        Quaternion spawnRot = Quaternion.LookRotation(target.position - attacker.position);

        //화염구 프리팹 생성
        GameObject projectileGO = Instantiate(_projectilePrefab, spawnPos, spawnRot);

        //생성된 화염구를 초기화
        ProjectileController controller = projectileGO.GetComponent<ProjectileController>();
        if (controller != null)
        {
            controller.Initialize(_projectileSpeed, _totalDamage, _duration, _tickInterval);
        }
    }
}