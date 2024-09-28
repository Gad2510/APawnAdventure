using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using Betadron.Managers;
namespace Betadron.Pawn
{
    public class Health : MonoBehaviour
    {
        private Character scp_char;
        private Slider sl_healthBar;
        private MeshRenderer mr_selfRender;

        // Start is called before the first frame update
        void Start()
        {
            Transform ui_canvas = ((GameModeGameplay)GameManager.gm_gamemode).UI_Characters.transform;
            GameObject go = Instantiate(Resources.Load("Prefabs/sl_character"), ui_canvas) as GameObject;
            scp_char = gameObject.GetComponent<Character>();
            sl_healthBar = go.GetComponent<Slider>();
            mr_selfRender = GetComponent<MeshRenderer>();

            UpdateHealthBarValue(scp_char.CharacterStats.GetHealthPorcentage());
        }

        // Update is called once per frame
        void Update()
        {
            //Acatuliza posicion de la barra de vida
            UpdateHealthBarPosition();
        }

        public void UpdateHealthBarValue(float _porcentage)
        {
            sl_healthBar.value = _porcentage;
        }

        private void UpdateHealthBarPosition()
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            sl_healthBar.transform.position = pos + (Vector3.down * 25);
        }

        private void ApplyDamage(Character _attacker)
        {
            int damage = _attacker.CharacterStats.attack;
            scp_char.CharacterStats.UpdateHealth(damage);
        }

        public void GetDamage(Character _attacker)
        {
            ApplyDamage(_attacker);
            float porcentageleft = scp_char.CharacterStats.GetHealthPorcentage();

            if (porcentageleft < 0)
            {
                porcentageleft = 0;
            }

            StartCoroutine(UpdateHealthBar(sl_healthBar.value, porcentageleft));
        }


        private IEnumerator UpdateHealthBar(float _from, float _to)
        {
            float dif = (_from - _to);
            float original;
            float counter = 0;

            Color currentColor = mr_selfRender.material.color;
            while (counter < 1)
            {
                counter += Time.deltaTime;
                mr_selfRender.material.color = Color.Lerp(currentColor, Color.red, Mathf.Cos(counter * Mathf.PI * 5f));
                original = _from - (counter * dif);
                sl_healthBar.value = original;
                yield return null;
            }
            mr_selfRender.material.color = currentColor;
            sl_healthBar.value = _to;
        }
    }
}