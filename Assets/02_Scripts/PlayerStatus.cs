using UnityEngine;

public class PlayerStatus : IMonsterDamageable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyBurn(float totalDamage, float duration, float tickInterval)
    {
        Debug.Log($"화상 시작! {duration}초 동안 총 {totalDamage}의 피해를 입습니다.");
    }

    public void TaekDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
}
