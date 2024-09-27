namespace Betadron.Struct
{
    [System.Serializable]
    public struct Stats
    {
        public string str_name;

        public int int_maxHealth;
        public int int_health;

        public int int_maxStamina;
        public int int_stamina;

        public int int_maxMovement;
        public int int_movement;

        public int attack;
        public int defence;
        public int dexerity;

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
    }
}