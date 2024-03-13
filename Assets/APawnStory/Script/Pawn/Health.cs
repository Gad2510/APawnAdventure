using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private Slider sl_healthBar;
    private MeshRenderer mr_selfRender;

    // Start is called before the first frame update
    void Awake()
    {
        Transform ui_canvas = GameObject.Find("UICharacters").transform;
        GameObject go = Instantiate(Resources.Load("Prefabs/sl_character"),ui_canvas) as GameObject;
        sl_healthBar = go.GetComponent<Slider>();
        mr_selfRender = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBarPosition();
    }

    public void InitHealthBar(float _porcentage)
    {
        sl_healthBar.value = _porcentage;
    }

    private void UpdateHealthBarPosition()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        sl_healthBar.transform.position =pos + (Vector3.down*25);
    }

    public void GetDamage(float _porcentageleft)
    {
        if (_porcentageleft < 0)
        {
            _porcentageleft = 0;
        }

        StartCoroutine(UpdateHealthBar(sl_healthBar.value, _porcentageleft));
    }


    private IEnumerator UpdateHealthBar(float _from, float _to)
    {
        float dif = (_from - _to);
        float original;
        float counter = 0;

        Color currentColor = mr_selfRender.material.color;
        while(counter < 1)
        {
            counter += Time.deltaTime;
            mr_selfRender.material.color = Color.Lerp(currentColor, Color.red, Mathf.Cos(counter * Mathf.PI*5f));
            original = _from-(counter * dif);
            sl_healthBar.value = original;
            yield return null;
        }
        mr_selfRender.material.color = currentColor;
        sl_healthBar.value = _to;
    }
}
