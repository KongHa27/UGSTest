using UnityEngine;

public class PlayerStatus : MonoBehaviour
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
        Debug.Log($"ȭ�� ����! {duration}�� ���� �� {totalDamage}�� ���ظ� �Խ��ϴ�.");
    }
}
