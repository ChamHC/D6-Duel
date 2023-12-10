using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum Abilities
{
    Empty, Joker, Leaf, Sun, Sickle, Snowflake, Cross,
    Star, Crown, Anchor, Pentacle, Goblet, Eagle
}

public class PowerUps : MonoBehaviour
{
    public GameController gameController;
    public TextMeshProUGUI abilityText;
    public Abilities ability;

    public void Update()
    {
        UpdateText();
    }

    public void ObtainPowerups(GameObject dice)
    {
        if (ability == Abilities.Empty)
        {
            if ( dice.tag == "Player")
                ability = (Abilities)Random.Range(1, 13);
            else if (dice.tag == "Opponent")
                ability = (Abilities)Random.Range(1, 12);
        }
    }

    public void UpdateText()
    {
        switch (ability)
        {
            case Abilities.Empty:
                abilityText.text = "BLANK";
                break;
            case Abilities.Joker:
                abilityText.text = "JOKER";
                break;
            case Abilities.Leaf:
                abilityText.text = "SPRING";
                break;
            case Abilities.Sun:
                abilityText.text = "SUMMER";
                break;
            case Abilities.Sickle:
                abilityText.text = "AUTUMN";
                break;
            case Abilities.Snowflake:
                abilityText.text = "WINTER";
                break;
            case Abilities.Cross:
                abilityText.text = "CROSSES";
                break;
            case Abilities.Star:
                abilityText.text = "STARS";
                break;
            case Abilities.Crown:
                abilityText.text = "CROWNS";
                break;
            case Abilities.Anchor:
                abilityText.text = "ANCHORS";
                break;
            case Abilities.Eagle:
                abilityText.text = "EAGLES";
                break;
            case Abilities.Pentacle:
                abilityText.text = "PENTACLE";
                break;
            case Abilities.Goblet:
                abilityText.text = "GOBLETS";
                break;
        }
    }

    public void PlayerCast()
    {
        if (gameController.state == GameState.PlayerTurn && gameController.AllStationary())
            CastAbility(gameController.players, gameController.opponents);
    }

    public void OpponentCast()
    {
        if (gameController.state == GameState.OpponentTurn && gameController.AllStationary())
            CastAbility(gameController.opponents, gameController.players);
    }

    public void CastAbility(List<GameObject> caster, List<GameObject> victim)
    {
        switch (ability)
        {
            case Abilities.Empty:
                break;
            case Abilities.Joker:
                JokerAbility();
                break;
            case Abilities.Leaf:
                LeafAbility(caster);
                break;
            case Abilities.Sun:
                SunAbility(caster);
                break;
            case Abilities.Sickle:
                SickleAbility(caster, victim);
                break;
            case Abilities.Snowflake:
                SnowflakeAbility(victim);
                break;
            case Abilities.Cross:
                CrossAbility(caster);
                break;
            case Abilities.Star:
                StarAbility(caster);
                break;
            case Abilities.Crown:
                CrownAbility(caster);
                break;
            case Abilities.Anchor:
                AnchorAbility(caster);
                break;
            case Abilities.Eagle:
                EagleAbility(caster);
                break;
            case Abilities.Pentacle:
                PentacleAbility();
                break;
            case Abilities.Goblet:
                GobletAbility(caster);
                break;
        }
        ability = Abilities.Empty;
        abilityText.text = "EMPTY";
    }

    public void JokerAbility()
    {
        Debug.Log("Switch All Dices Position and Value");
        ShuffleList(gameController.players);
        ShuffleList(gameController.opponents);

        for (int i = 0; i < gameController.players.Count; i++)
        {
            GameObject opponentDie = gameController.opponents[i];

            Vector3 tempPosition = gameController.players[i].transform.position;
            gameController.players[i].transform.position = opponentDie.transform.position;
            opponentDie.transform.position = tempPosition;

            float tempValue = gameController.players[i].GetComponent<UnitObject>().diceValue;
            gameController.players[i].GetComponent<UnitObject>().diceValue = opponentDie.GetComponent<UnitObject>().diceValue;
            opponentDie.GetComponent<UnitObject>().diceValue = tempValue;
        }

        void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    public void LeafAbility(List<GameObject> list)
    {
        Debug.Log("Increase Dice Size");
        foreach (var item in list)
        {
            item.transform.localScale *= 2.0f;
        }
    }

    public void SunAbility(List<GameObject> list)
    {
        Debug.Log("Evey dices from List<GameObject> list will push surrounding dices outward on landing");
        foreach (var item in list)
        {
            item.GetComponent<UnitObject>().sunAbility = true;
        }
    }

    public void SickleAbility(List<GameObject> caster, List<GameObject> victim)
    {
        Debug.Log("Convert a random opponent dice to team dice");
        
        int victimIndex = Random.Range(0, victim.Count);
        GameObject targetDice = victim[victimIndex];

        Vector3 position = targetDice.transform.position;
        Quaternion rotation = targetDice.transform.rotation;

        Destroy(targetDice);
        GameObject newDice = Instantiate(caster[0], position, rotation);

        victim.RemoveAt(victimIndex);
        caster.Add(newDice);
    }

    public void SnowflakeAbility(List<GameObject> victim)
    {
        Debug.Log("Reduce enemy dices force");
        foreach (var item in victim)
        {
            item.GetComponent<UnitObject>().isSnowflake = true;
        }
    }

    public void CrossAbility(List<GameObject> caster)
    {
        Debug.Log("Increase player dices force");
        foreach (var item in caster)
        {
            item.GetComponent<UnitObject>().crossAbility = true;
        }
    }

    public void StarAbility(List<GameObject> caster)
    {
        Debug.Log("Spawn In A New Dice");
        GameObject newDice = Instantiate(caster[0], new (0f, 25f, 0.5f), Quaternion.identity);
        gameController.Setup();
    }

    public void CrownAbility(List<GameObject> caster)
    {
        Debug.Log("Grant a barrier that push away opponent once on next round");
        foreach (var item in caster)
        {
            item.GetComponent<UnitObject>().crownAbility = true;
        }
    }

    public void AnchorAbility(List<GameObject> caster)
    {
        Debug.Log("Skip enemy turn.");
        foreach (var item in caster)
        {
            item.GetComponent<UnitObject>().anchorAbility = true;
        }
    }

    public void EagleAbility(List<GameObject> caster)
    {
        Debug.Log("Allow player dragging to control launch distance");
        foreach (var item in caster)
        {
            item.GetComponent<UnitObject>().eagleAbility = true;
        }
    }

    public void PentacleAbility()
    {
        Debug.Log("Rethrow all dices");
        float force = 100.0f;

        foreach (List<GameObject> diceList in new List<List<GameObject>> { gameController.players, gameController.opponents, gameController.abilities })
        {
            foreach (GameObject dice in diceList)
            {
                dice.GetComponent<Rigidbody>().AddForce(Vector3.up * force, ForceMode.Impulse);

                Vector3 torque = Vector3.Cross(new Vector3(Random.value, 0f, Random.value), Vector3.up);
                dice.GetComponent<Rigidbody>().AddTorque(torque * 10f, ForceMode.Impulse);
                dice.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            }
        }
    }

    public void GobletAbility(List<GameObject> caster)
    {
        Debug.Log("Change a dice value of 1-6 to 3-8");
        foreach(var dice in caster)
        {
            if (!dice.GetComponent<UnitObject>().gobletAbility)
            {
                dice.GetComponent<UnitObject>().gobletAbility = true;

                if (dice.tag == "Player")
                    dice.GetComponent<MeshRenderer>().material = gameController.gobletPlayerMaterial;
                else if (dice.tag == "Opponent")
                    dice.GetComponent<MeshRenderer>().material = gameController.gobletOpponentMaterial;

                return;
            }
        }
    }
}
