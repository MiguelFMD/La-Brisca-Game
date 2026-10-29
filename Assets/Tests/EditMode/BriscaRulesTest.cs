using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class BriscaRulesTests
{
    // --- HELPER: Crea cartas en memoria y asigna campos privados ---
    private CardData CreateCard(Suit suit, Rank rank)
    {
        CardData card = ScriptableObject.CreateInstance<CardData>();
        
        // Usamos Reflection para inyectar valores en los campos privados [SerializeField]
        FieldInfo suitField = typeof(CardData).GetField("cardSuit", BindingFlags.NonPublic | BindingFlags.Instance);
        suitField.SetValue(card, suit);
        
        FieldInfo rankField = typeof(CardData).GetField("cardRank", BindingFlags.NonPublic | BindingFlags.Instance);
        rankField.SetValue(card, rank);
        
        return card;
    }

    // --- TESTS DE REGLAS DE BAZA (CalculateTrickWinner) ---

    [Test]
    public void Baza_GanaTriunfo_ContraCartaMayorDeOtroPalo()
    {
        // Arrange (Preparación)
        PlayerState p1 = new PlayerState(100f);
        PlayerState p2 = new PlayerState(100f);
        
        CardData cartaTriunfoData = CreateCard(Suit.Golds, Rank.Two); // Triunfo de valor bajo
        CardData cartaNormalAltaData = CreateCard(Suit.Cups, Rank.Ace); // As de otro palo
        
        CardStatus cardStatusTriunfo = new CardStatus(cartaTriunfoData, p1);
        CardStatus cartaNormalAlta = new CardStatus(cartaNormalAltaData, p2);

        p1.Hand.Add(cardStatusTriunfo);
        p1.PlayCard(cardStatusTriunfo);
        
        p2.Hand.Add(cartaNormalAlta);
        p2.PlayCard(cartaNormalAlta);

        // Act (Ejecución)
        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Golds, p1, p2);

        // Assert (Comprobación)
        Assert.That(result.winner, Is.EqualTo(p1), "El triunfo debería ganar aunque sea de menor rango.");

        // Limpieza de memoria
        Object.DestroyImmediate(cartaTriunfoData);
        Object.DestroyImmediate(cartaNormalAltaData);
    }

    [Test]
    public void Baza_MismoPalo_GanaRangoMayor()
    {
        PlayerState p1 = new PlayerState(100f);
        PlayerState p2 = new PlayerState(100f);
        
        CardData cartaBaja = CreateCard(Suit.Swords, Rank.Three);
        CardData cartaAlta = CreateCard(Suit.Swords, Rank.Ace);

        CardStatus baja = new CardStatus(cartaBaja, p1);
        CardStatus alta = new CardStatus(cartaAlta, p2);

        p1.Hand.Add(baja);
        p1.PlayCard(baja);
        
        p2.Hand.Add(alta);
        p2.PlayCard(alta);

        // Act (El triunfo es Bastos, pero juegan Espadas)
        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Clubs, p1, p2);

        // Assert (En tu Enum, Ace va después de Three, por lo que su valor numérico es mayor)
        Assert.That(result.winner, Is.EqualTo(p2), "A mismo palo, debería ganar el rango mayor (As sobre Tres).");

        Object.DestroyImmediate(cartaBaja);
        Object.DestroyImmediate(cartaAlta);
    }

    [Test]
    public void Baza_DistintoPalo_SinTriunfo_GanaElQueAbre()
    {
        PlayerState p1 = new PlayerState(100f);
        PlayerState p2 = new PlayerState(100f);
        
        CardData cartaAbre = CreateCard(Suit.Cups, Rank.Two); // Abre con Copas
        CardData cartaResponde = CreateCard(Suit.Swords, Rank.Ace); // Responde con Espadas (As)
        
        CardStatus abre = new CardStatus(cartaAbre, p1);
        CardStatus responde = new CardStatus(cartaResponde, p2);

        p1.Hand.Add(abre);
        p1.PlayCard(abre);
        
        p2.Hand.Add(responde);
        p2.PlayCard(responde);

        // Act (El triunfo es Oros)
        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Golds, p1, p2);

        // Assert
        Assert.That(result.winner, Is.EqualTo(p1), "Si nadie tira triunfo y no asisten al palo, gana siempre el que tiró primero.");

        Object.DestroyImmediate(cartaAbre);
        Object.DestroyImmediate(cartaResponde);
    }

    // --- TESTS DE FIN DE PARTIDA (CheckGameOverConditions) ---

    [Test]
    public void FinPartida_EsVerdadero_CuandoVidaLlegaACero()
    {
        // Arrange
        PlayerState p1 = new PlayerState(100f);
        PlayerState p2 = new PlayerState(100f);

        // Simulamos que tienen cartas para que esa no sea la causa del fin
        CardData dummyCard = CreateCard(Suit.Golds, Rank.Two);
        CardStatus dummy = new CardStatus(dummyCard, p1);
        p1.Hand.Add(dummy);
        p2.Hand.Add(dummy);

        // Act 1: Nadie ha muerto
        bool gameOverAntes = BriscaRules.CheckGameOverConditions(p1, p2);

        // Act 2: Matamos a P2
        p2.TakeDamage(150f);
        bool gameOverDespues = BriscaRules.CheckGameOverConditions(p1, p2);

        // Assert
        Assert.IsFalse(gameOverAntes, "El juego no debería terminar si ambos tienen vida y cartas.");
        Assert.IsTrue(gameOverDespues, "El juego debe terminar si un jugador tiene vida <= 0.");

        Object.DestroyImmediate(dummyCard);
    }

    [Test]
    public void FinPartida_EsVerdadero_CuandoUnJugadorSeQuedaSinCartas()
    {
        // Arrange
        PlayerState p1 = new PlayerState(100f); // P1 se queda sin cartas (Hand y Deck vacíos por defecto)
        PlayerState p2 = new PlayerState(100f);
        
        // Le damos una carta a P2 para que solo P1 esté sin cartas
        CardData dummyCard = CreateCard(Suit.Golds, Rank.Two);
        CardStatus dummy = new CardStatus(dummyCard, p2);
        p2.Hand.Add(dummy);

        // Act
        bool isGameOver = BriscaRules.CheckGameOverConditions(p1, p2);

        // Assert
        Assert.IsTrue(isGameOver, "El juego debe terminar si al menos un jugador se queda sin cartas en mano y mazo.");

        Object.DestroyImmediate(dummyCard);
    }
}