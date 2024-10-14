namespace Betadron.Struct
{
    [System.Serializable]
    public struct Stats
    {
        public string str_name;
        //Vida del caracter
        public int int_maxHealth;
        public int int_health;
        //Energia para realizar una accion
        //Energia disponible antes de sufrir por hambre
        public int int_maxStamina;
        public int int_stamina;
        //Cantidad de pasos que puede dar en cada turno
        public int int_maxMovement;
        public int int_movement;
        //Poder de ataque
        public int attack;
        //Armadura del caracter
        public int defense;
        //Capacidad de esquivar un movimiento 
        public int dexerity;
        //Es un elemento controlado por el jugador
        public bool isPlayer;
        public float GetHealthPorcentage()
        {
            return (float)int_health / (float)int_maxHealth;
        }

        public void UpdateHealth(int _amount)
        {
            int_health -= _amount;
        }

        public void UpdateStamina(int _amount)
        {
            int_stamina -= _amount;
        }
        public void UpdateMovement(int _amount)
        {
            int_movement -= _amount;
        }
    }
}