using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Abilities
{
    Empty, Joker, Leaf, Sun, Sickle, Snowflake, Cross,
    Star, Crown, Anchor, Eagle, Pentacle, Goblet
}

public class PowerUps : MonoBehaviour
{
    public GameController gameController;
    public TextMeshProUGUI abilityText;
    public Abilities ability;

    public void ObtainPowerups()
    {
        if (ability == Abilities.Empty)
            ability = (Abilities)Random.Range(1, 13);

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

    public void CastAbility()
    {
        switch (ability)
        {
            case Abilities.Empty:
                break;
            case Abilities.Joker:
                JokerAbility();
                break;
            case Abilities.Leaf:
                LeafAbility();
                break;
            case Abilities.Sun:
                SunAbility();
                break;
            case Abilities.Sickle:
                SickleAbility();
                break;
            case Abilities.Snowflake:
                SnowflakeAbility();
                break;
            case Abilities.Cross:
                CrossAbility();
                break;
            case Abilities.Star:
                StarAbility();
                break;
            case Abilities.Crown:
                CrownAbility();
                break;
            case Abilities.Anchor:
                AnchorAbility();
                break;
            case Abilities.Eagle:
                EagleAbility();
                break;
            case Abilities.Pentacle:
                PentacleAbility();
                break;
            case Abilities.Goblet:
                GobletAbility();
                break;
        }
        ability = Abilities.Empty;
        abilityText.text = "EMPTY";
    }

    public void JokerAbility()
    {
        Debug.Log("Switch All Dices Position and Value");
    }

    public void LeafAbility()
    {
        Debug.Log("Increase Dice Size");
    }

    public void SunAbility()
    {
        Debug.Log("Apply Forces To Surrounding On Landing (Explosion)");
    }

    public void SickleAbility()
    {
        Debug.Log("Convert a random opponent dice to player dice");
    }

    public void SnowflakeAbility()
    {
        Debug.Log("Reduce enemy dices force");
    }

    public void CrossAbility()
    {
        Debug.Log("Increase player dices force");
    }

    public void StarAbility()
    {
        Debug.Log("Swap two dices position");
    }

    public void CrownAbility()
    {
        Debug.Log("Dices grant invincibility");
    }

    public void AnchorAbility()
    {
        Debug.Log("Skip enemy turn.");
    }


    public void EagleAbility()
    {
        Debug.Log("Allow player dragging to control launch distance");
    }

    public void PentacleAbility()
    {
        Debug.Log("Rethrow all dices");
    }

    public void GobletAbility()
    {
        Debug.Log("Spawn in a dice with higher values");
    }

    public void PlayerCast()
    {
        if (gameController.state == GameState.PlayerTurn)
            CastAbility();
    }
}
