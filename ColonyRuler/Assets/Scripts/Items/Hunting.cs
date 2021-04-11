using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

/// <summary>
/// Hunting class. 
/// Each day should be hunted into different animals
/// </summary>
class Hunting : GameMaterial
{
    public int m_squdres = 1;

    void AnimalHunting(long workers)
    {
        int count = 0;
        WildAnimal ani = WildAnimal.FindSomeone(out count);
        int killedAnimals = 0;
        // prepare
        float speed = 1;
        float attackDistance = 1;
        float attack = 1f;
        float protection = 0;
        float distance = 10;

        //calc attributes
        foreach(var tool in m_tools)
        {
            foreach(var effectPair in tool.m_toolLink.m_effects)
            {
                var effect = effectPair.Value;
                switch (effect.m_name)
                {
                    case "hunt":
                        attack += (1 + tool.m_value);
                        break;
                    case "protection":
                        protection += tool.m_value;
                        break;
                    case "attack_distance":
                        if (effect.m_value > attackDistance)
                            attackDistance = effect.m_value;
                        break;
                }
            }
        }

        if (attackDistance > distance)
            distance = attackDistance;

        bool scared = false;
        float aniHealth = ani.m_protection * count;
        float squadreHealth = workers * 10;
        long hunters = workers;

        // fight!
        while(killedAnimals < count && !(scared && distance > attackDistance && ani.m_speed > speed) 
            && squadreHealth > 0)
        {
            //attack animals
            if (distance <= attackDistance)
            {
                aniHealth -= attack * workers;
                killedAnimals = count - (int)((aniHealth / ani.m_protection) + (float.Equals(aniHealth % ani.m_protection, 0) ? 0f : 1f));
            }
            // animals attack
            if(distance <= 1)
            {
                float aniAttack = ani.m_attack * (count - killedAnimals) - protection * workers;
                if (aniAttack > 0)
                {
                    squadreHealth -= aniAttack;
                    workers = (int)(squadreHealth / 10f + (squadreHealth % 10f > 0f ? 1f : 0f));
                }
            }
            scared = (count - killedAnimals) < ani.m_scary;

            distance += -speed + (scared ? 1 : -1) * ani.m_speed;
            if (distance < 0)
                distance = 0;
        }

        //how many hunters was killed
        long killedWorkers = hunters - workers;
        m_workers -= killedWorkers;
        Storage.m_storage.m_people.PeopleNumber -= killedWorkers;

        ani.m_count -= killedAnimals;

        for (int it = 0; it < ani.m_butcheringPerPerson.Length; it++)
        {
            var dep = ani.m_butcheringPerPerson[it];
            {
                for (int i = 0; i < dep.m_dependency.Count; i++)
                {
                    dep.m_dependency[i].m_count += dep.m_value[i] * killedAnimals;
                    GameMaterial mat = dep.m_dependency[i] as GameMaterial;
                    if (mat != null)
                    {
                        float maxCount = ((float)mat.StorageSize / mat.m_size);
                        if (dep.m_dependency[i].m_count > maxCount)
                            dep.m_dependency[i].m_count = maxCount;
                    }
                }
            }
        }
    }


    public override void Working(long worked = 0)
    {
        if (m_workers > worked)
        {
            GettingTools();
            long workersInSquadre = m_workers / m_squdres;
            AnimalHunting(workersInSquadre + m_workers % m_squdres);
            for (int i = 1; i < m_squdres; i++)
                AnimalHunting(workersInSquadre);
        }
    }
}

