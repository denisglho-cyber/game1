using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthBarFill;
    [SerializeField] private float _reduceSpeed = 5f;

    private void Update()
    {
        float targetFill = 0f;

        if (Player.Instance != null)
        {
            targetFill = Player.Instance.GetHealthPercent();
        }

        _healthBarFill.fillAmount = Mathf.Lerp(_healthBarFill.fillAmount, targetFill, Time.deltaTime * _reduceSpeed);
    }
}