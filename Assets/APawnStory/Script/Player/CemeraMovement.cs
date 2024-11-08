using UnityEngine;

namespace Betadron.Player
{
    public class CemeraMovement : MonoBehaviour
    {
        private KeyCode k_scrollKey= KeyCode.LeftShift;

        [SerializeField]
        private float f_screenPorcentage=0.15f;
        [SerializeField]
        private float f_speed = 0.1f;
        private Vector2 v2_screenSize;

        public Vector3 v3_origin;
        public Vector2 pos;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            v2_screenSize = new Vector2(Screen.width, Screen.height)* f_screenPorcentage;
            v3_origin = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(k_scrollKey))
            {
                Scroll();
            }
        }

        public void Scroll()
        {
            Vector2 mousePos = Input.mousePosition;

            pos = mousePos;

            Vector3 dir=Vector3.zero;

            if (v2_screenSize.x > mousePos.x)
            {
                dir += Vector3.left;
            }
            else if ((Screen.width-v2_screenSize.x) < mousePos.x)
            {
                dir += Vector3.right;
            }

            if (v2_screenSize.y > mousePos.y)
            {
                dir += Vector3.back;
            }
            else if ((Screen.height - v2_screenSize.y) < mousePos.y)
            {
                dir += Vector3.forward;
            }
            dir = Vector3.Normalize(dir);
            transform.position += (dir * f_speed * Time.deltaTime);
        }


    }
}
