using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class CombatPowersTests
{
    private PlayerState winner;
    private PlayerState loser;
    private List<UnityEngine.Object> tempObjects;

    [SetUp]
    public void SetUp()
    {
        tempObjects = new List<UnityEngine.Object>();
        winner = new PlayerState(100f);
        loser = new PlayerState(100f);
    }

    [TearDown]
    public void TearDown()
    {
        for (int i = tempObjects.Count - 1; i >= 0; i--)
        {
            if (tempObjects[i] != null)
            {
                UnityEngine.Object.DestroyImmediate(tempObjects[i]);
            }
        }
    }

    #region Helpers

    private CardData CreateCard(Suit suit, Rank rank = Rank.Two)
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        tempObjects.Add(card);

        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(CardData).GetField("cardSuit", flags)?.SetValue(card, suit);
        typeof(CardData).GetField("cardRank", flags)?.SetValue(card, rank);
        return card;
    }

    private void PopulateDummyScriptableObjects(Component component)
    {
        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        foreach (var field in component.GetType().GetFields(flags))
        {
            if (typeof(ScriptableObject).IsAssignableFrom(field.FieldType) && field.GetValue(component) == null)
            {
                var dummySo = ScriptableObject.CreateInstance(field.FieldType);
                tempObjects.Add(dummySo);
                field.SetValue(component, dummySo);
            }
        }
    }

    private PlayerState CreatePlayerWithDeck(int cardCount)
    {
        var go = new GameObject("TestPlayer");
        tempObjects.Add(go);

        var hand = go.AddComponent<Hand>();
        PopulateDummyScriptableObjects(hand);
        typeof(Hand).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(hand, null);

        var player = go.AddComponent<Player>();
        PopulateDummyScriptableObjects(player);
        typeof(Player).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(player, null);

        var deckType = ScriptableObject.CreateInstance<DeckType>();
        tempObjects.Add(deckType);
        deckType.cards = new CardData[cardCount];
        for (int i = 0; i < cardCount; i++)
        {
            deckType.cards[i] = CreateCard(Suit.Swords, Rank.Two);
        }

        player.state.Deck.AssignDeck(deckType);
        player.state.Deck.CreateDeck(player);
        return player.state;
    }

    private Hand CreateHandComponent()
    {
        var go = new GameObject("TestHand");
        tempObjects.Add(go);

        var hand = go.AddComponent<Hand>();
        PopulateDummyScriptableObjects(hand);
        typeof(Hand).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)?.Invoke(hand, null);
        return hand;
    }

    private CardData PreparePlayedCard(PlayerState player, Suit suit, Rank rank = Rank.Two)
    {
        CardData cardData = CreateCard(suit, rank);
        player.PlayedCard = new CardStatus(cardData, player);
        return cardData;
    }

    #endregion

    #region 1. Valores de Cartas (BriscaRules.CalculateCardValue)

    [TestCase(Rank.Two, 1)]
    [TestCase(Rank.Four, 1)]
    [TestCase(Rank.Five, 1)]
    [TestCase(Rank.Six, 1)]
    [TestCase(Rank.Seven, 1)]
    [TestCase(Rank.Jack, 2)]
    [TestCase(Rank.Knight, 3)]
    [TestCase(Rank.King, 4)]
    [TestCase(Rank.Three, 10)]
    [TestCase(Rank.Ace, 11)]
    public void BriscaRules_CalculateCardValue_ReturnsExpectedPointsForAllRanks(Rank rank, int expectedValue)
    {
        CardData card = CreateCard(Suit.Swords, rank);

        int actualValue = BriscaRules.CalculateCardValue(card);

        Assert.AreEqual(expectedValue, actualValue);
    }

    #endregion

    #region 2. Resolución de Baza (BriscaRules.CalculateTrickWinner)

    [Test]
    public void TrickWinner_ExactSameSuitAndRank_ReturnsNullAsDraw()
    {
        PreparePlayedCard(winner, Suit.Swords, Rank.Ace);
        PreparePlayedCard(loser, Suit.Swords, Rank.Ace);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Swords, winner, loser);

        Assert.IsNull(result);
    }

    [Test]
    public void TrickWinner_TriumphPlayedBySecondPlayer_BeatsHigherNonTriumphFirstPlayer()
    {
        // Jugador 1 sale con As de Espadas (11 pts), Jugador 2 responde con Dos de Copas (Triunfo)
        PreparePlayedCard(winner, Suit.Swords, Rank.Ace);
        PreparePlayedCard(loser, Suit.Cups, Rank.Two);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Cups, winner, loser);

        Assert.IsNotNull(result);
        Assert.AreEqual(loser, result.winner);
        Assert.AreEqual(winner, result.loser);
    }

    [Test]
    public void TrickWinner_TriumphPlayedByFirstPlayer_BeatsHigherNonTriumphSecondPlayer()
    {
        PreparePlayedCard(winner, Suit.Golds, Rank.Two);
        PreparePlayedCard(loser, Suit.Clubs, Rank.Ace);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Golds, winner, loser);

        Assert.IsNotNull(result);
        Assert.AreEqual(winner, result.winner);
        Assert.AreEqual(loser, result.loser);
    }

    [Test]
    public void TrickWinner_BothPlayTriumph_HigherRankWins()
    {
        // Tres (10) gana a Rey (4), y As (11) gana a Tres (10)
        PreparePlayedCard(winner, Suit.Clubs, Rank.King);
        PreparePlayedCard(loser, Suit.Clubs, Rank.Three);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Clubs, winner, loser);

        Assert.AreEqual(loser, result.winner);
        Assert.AreEqual(winner, result.loser);
    }

    [Test]
    public void TrickWinner_NeitherPlaysTriumph_SameSuit_HigherRankWins()
    {
        PreparePlayedCard(winner, Suit.Swords, Rank.Ace);
        PreparePlayedCard(loser, Suit.Swords, Rank.Three);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Cups, winner, loser);

        Assert.AreEqual(winner, result.winner);
        Assert.AreEqual(loser, result.loser);
    }

    [Test]
    public void TrickWinner_NeitherPlaysTriumph_DifferentNonTriumphSuit_FirstPlayerWinsByTrickSuit()
    {
        // Jugador 1 marca el palo de salida con Dos de Bastos; Jugador 2 tira As de Espadas (ni triunfo ni palo de salida)
        PreparePlayedCard(winner, Suit.Clubs, Rank.Two);
        PreparePlayedCard(loser, Suit.Swords, Rank.Ace);

        TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Golds, winner, loser);

        Assert.AreEqual(winner, result.winner);
        Assert.AreEqual(loser, result.loser);
    }

    [Test]
    public void TrickWinner_FullRankHierarchy_IsOrderedCorrectly()
    {
        Rank[] ascendingRanks =
        {
            Rank.Two, Rank.Four, Rank.Five, Rank.Six, Rank.Seven,
            Rank.Jack, Rank.Knight, Rank.King, Rank.Three, Rank.Ace
        };

        for (int i = 0; i < ascendingRanks.Length - 1; i++)
        {
            PreparePlayedCard(winner, Suit.Swords, ascendingRanks[i]);
            PreparePlayedCard(loser, Suit.Swords, ascendingRanks[i + 1]);

            TrickResult result = BriscaRules.CalculateTrickWinner(Suit.Golds, winner, loser);

            Assert.AreEqual(loser, result.winner, $"{ascendingRanks[i + 1]} debería ganar a {ascendingRanks[i]}");
        }
    }

    #endregion

    #region 3. Reglas Base de Palos (Ganar y Perder)

    [Test]
    public void Swords_Winner_DealsDamageToLoser()
    {
        PreparePlayedCard(winner, Suit.Swords, Rank.Three); // 10 de daño

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(90f, loser.CurrentHealth);
    }

    [Test]
    public void Swords_Loser_AppliesBleedingToWinner()
    {
        PreparePlayedCard(loser, Suit.Swords, Rank.King); // 4 stacks

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(1, winner.activeStatusEffects.Count);
        Assert.IsInstanceOf<BleedingEffect>(winner.activeStatusEffects[0]);
        Assert.AreEqual(4, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Clubs_Winner_AddsShieldToWinner()
    {
        PreparePlayedCard(winner, Suit.Clubs, Rank.Ace); // 11 de escudo

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(11f, winner.CurrentShield);
    }

    [Test]
    public void Clubs_Loser_AppliesThornsToLoser()
    {
        PreparePlayedCard(loser, Suit.Clubs, Rank.Knight); // 3 stacks

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(1, loser.activeStatusEffects.Count);
        Assert.IsInstanceOf<ThornsEffect>(loser.activeStatusEffects[0]);
        Assert.AreEqual(3, loser.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Cups_Winner_HealsWinner()
    {
        winner.TakeDamage(new DamageContext { Amount = 40f });
        PreparePlayedCard(winner, Suit.Cups, Rank.Ace); // Cura 11

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(71f, winner.CurrentHealth);
    }

    [Test]
    public void Cups_Loser_ClearsAllActiveStatusEffects()
    {
        loser.AddStatusEffect(new BleedingEffect(4));
        loser.AddStatusEffect(new ThornsEffect(5));
        PreparePlayedCard(loser, Suit.Cups, Rank.Two);

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(0, loser.activeStatusEffects.Count);
    }

    [Test]
    public void Cups_Loser_PreservesAccumulatedInvestment()
    {
        loser.AddInvestment(6);
        PreparePlayedCard(loser, Suit.Cups, Rank.Two);

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(6, loser.Investment);
    }

    [Test]
    public void Golds_Winner_BurnsCardsFromLoserDeck()
    {
        loser = CreatePlayerWithDeck(20);
        PreparePlayedCard(winner, Suit.Golds, Rank.Three); // Quema 10 cartas

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(10, loser.Deck.CountCards());
    }

    [Test]
    public void Golds_Loser_AddsInvestmentToLoser()
    {
        PreparePlayedCard(loser, Suit.Golds, Rank.Ace); // Invierte 11

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(11, loser.Investment);
    }

    #endregion

    #region 4. Escalado y Ciclo de Investment

    [Test]
    public void Investment_AmplifiesDamage_AndResetsToZero()
    {
        winner.AddInvestment(7);
        PreparePlayedCard(winner, Suit.Swords, Rank.Three); // 10 + 7 = 17

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(83f, loser.CurrentHealth);
        Assert.AreEqual(0, winner.Investment);
    }

    [Test]
    public void Investment_AmplifiesBleedingStacks_AndResetsToZero()
    {
        loser.AddInvestment(4);
        PreparePlayedCard(loser, Suit.Swords, Rank.Jack); // 2 + 4 = 6

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(6, winner.activeStatusEffects[0].Stacks);
        Assert.AreEqual(0, loser.Investment);
    }

    [Test]
    public void Investment_AmplifiesShield_AndResetsToZero()
    {
        winner.AddInvestment(6);
        PreparePlayedCard(winner, Suit.Clubs, Rank.King); // 4 + 6 = 10

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(10f, winner.CurrentShield);
        Assert.AreEqual(0, winner.Investment);
    }

    [Test]
    public void Investment_AmplifiesThornsStacks_AndResetsToZero()
    {
        loser.AddInvestment(5);
        PreparePlayedCard(loser, Suit.Clubs, Rank.Knight); // 3 + 5 = 8

        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(8, loser.activeStatusEffects[0].Stacks);
        Assert.AreEqual(0, loser.Investment);
    }

    [Test]
    public void Investment_AmplifiesHeal_AndResetsToZero()
    {
        winner.TakeDamage(new DamageContext { Amount = 50f });
        winner.AddInvestment(8);
        PreparePlayedCard(winner, Suit.Cups, Rank.Three); // 10 + 8 = 18

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(68f, winner.CurrentHealth);
        Assert.AreEqual(0, winner.Investment);
    }

    [Test]
    public void Investment_AmplifiesBurnCards_AndResetsToZero()
    {
        loser = CreatePlayerWithDeck(30);
        winner.AddInvestment(3);
        PreparePlayedCard(winner, Suit.Golds, Rank.Ace); // 11 + 3 = 14

        CombatManager.ApplyWinnerPower(winner, loser);

        Assert.AreEqual(16, loser.Deck.CountCards());
        Assert.AreEqual(0, winner.Investment);
    }

    [Test]
    public void Investment_ChainsAcrossConsecutiveGoldsLosses()
    {
        PreparePlayedCard(loser, Suit.Golds, Rank.King); // +4
        CombatManager.ApplyLoserPower(loser, winner);
        Assert.AreEqual(4, loser.Investment);

        PreparePlayedCard(loser, Suit.Golds, Rank.Three); // +10 -> 14
        CombatManager.ApplyLoserPower(loser, winner);

        Assert.AreEqual(14, loser.Investment);
    }

    #endregion

    #region 5. Daño, Escudo, Curación, Muerte y Eventos

    [Test]
    public void Shield_AbsorbsFullDamage_WithoutReducingHealthOrFiringHealthEvent()
    {
        winner.AddShield(20f);
        bool healthChangedFired = false;
        winner.onPlayerHealthChanged += () => healthChangedFired = true;

        winner.TakeDamage(new DamageContext { Amount = 12f });

        Assert.AreEqual(8f, winner.CurrentShield);
        Assert.AreEqual(100f, winner.CurrentHealth);
        Assert.IsFalse(healthChangedFired);
    }

    [Test]
    public void Shield_AbsorbsPartialDamage_AndRemainderDamagesHealth()
    {
        winner.AddShield(10f);
        bool healthChangedFired = false;
        winner.onPlayerHealthChanged += () => healthChangedFired = true;

        winner.TakeDamage(new DamageContext { Amount = 25f });

        Assert.AreEqual(0f, winner.CurrentShield);
        Assert.AreEqual(85f, winner.CurrentHealth);
        Assert.IsTrue(healthChangedFired);
    }

    [Test]
    public void Shield_StacksAndResetsProperly()
    {
        winner.AddShield(5f);
        winner.AddShield(7f);
        Assert.AreEqual(12f, winner.CurrentShield);

        winner.ResetShield();
        Assert.AreEqual(0f, winner.CurrentShield);
    }

    [Test]
    public void Heal_NeverExceedsMaxHealth()
    {
        winner.TakeDamage(new DamageContext { Amount = 5f });
        winner.Heal(50f);

        Assert.AreEqual(100f, winner.CurrentHealth);
    }

    [Test]
    public void LethalDamage_SetsIsDead_FiresDeathEvent_AndBlocksFurtherDamageOrHealing()
    {
        int deathEventCount = 0;
        winner.onPlayerDeath += () => deathEventCount++;

        winner.TakeDamage(new DamageContext { Amount = 150f });

        Assert.IsTrue(winner.IsDead);
        Assert.AreEqual(0f, winner.CurrentHealth);
        Assert.AreEqual(1, deathEventCount);

        winner.TakeDamage(new DamageContext { Amount = 20f });
        winner.Heal(50f);

        Assert.AreEqual(0f, winner.CurrentHealth);
        Assert.AreEqual(1, deathEventCount);
    }

    #endregion

    #region 6. Ciclo de Vida de BleedingEffect (Herida) y ThornsEffect (Espinas)

    [Test]
    public void Bleeding_Deals1DamagePerCardDrawn_AndConsumesStacksUntilDepleted()
    {
        winner = CreatePlayerWithDeck(5);
        winner.AddStatusEffect(new BleedingEffect(2));

        // 1er robo: queda 1 stack, 99 HP
        winner.DrawOneCard();
        Assert.AreEqual(99f, winner.CurrentHealth);
        Assert.AreEqual(1, winner.activeStatusEffects.Count);
        Assert.AreEqual(1, winner.activeStatusEffects[0].Stacks);

        // 2do robo: llega a 0 stacks, hace 1 de daño (98 HP) y TakeDamage lo elimina de la lista al instante
        winner.DrawOneCard();
        Assert.AreEqual(98f, winner.CurrentHealth);
        Assert.AreEqual(0, winner.activeStatusEffects.Count);

        // 3er robo: ya no hay Herida activa, no hace daño extra
        winner.DrawOneCard();
        Assert.AreEqual(98f, winner.CurrentHealth);
        Assert.AreEqual(0, winner.activeStatusEffects.Count);
    }
    
    [Test]
    public void Bleeding_IsAbsorbedByShield()
    {
        winner = CreatePlayerWithDeck(3);
        winner.AddShield(5f);
        winner.AddStatusEffect(new BleedingEffect(2));

        winner.DrawOneCard();

        Assert.AreEqual(4f, winner.CurrentShield);
        Assert.AreEqual(100f, winner.CurrentHealth);
        Assert.AreEqual(1, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Bleeding_DoesNotTrigger_WhenDeckIsEmpty()
    {
        winner = CreatePlayerWithDeck(0);
        winner.AddStatusEffect(new BleedingEffect(3));

        CardStatus drawn = winner.DrawOneCard();

        Assert.IsNull(drawn);
        Assert.AreEqual(100f, winner.CurrentHealth);
        Assert.AreEqual(3, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Thorns_RetaliatesFullStacks_AndClearsItselfOnFirstHit()
    {
        loser.AddStatusEffect(new ThornsEffect(8));

        loser.TakeDamage(new DamageContext
        {
            Source = winner,
            Target = loser,
            Amount = 3f,
            IsRetaliation = false
        });

        Assert.AreEqual(92f, winner.CurrentHealth);
        Assert.AreEqual(97f, loser.CurrentHealth);
        Assert.AreEqual(0, loser.activeStatusEffects[0].Stacks);

        loser.TakeDamage(new DamageContext
        {
            Source = winner,
            Target = loser,
            Amount = 3f,
            IsRetaliation = false
        });

        Assert.AreEqual(92f, winner.CurrentHealth);
        Assert.AreEqual(0, loser.activeStatusEffects.Count);
    }

    [Test]
    public void Thorns_BothPlayersWithThorns_DoesNotCauseInfiniteLoop()
    {
        winner.AddStatusEffect(new ThornsEffect(5));
        loser.AddStatusEffect(new ThornsEffect(7));

        loser.TakeDamage(new DamageContext
        {
            Source = winner,
            Target = loser,
            Amount = 10f,
            IsRetaliation = false
        });

        Assert.AreEqual(90f, loser.CurrentHealth);
        Assert.AreEqual(93f, winner.CurrentHealth);
        Assert.AreEqual(5, winner.activeStatusEffects[0].Stacks);
        Assert.AreEqual(0, loser.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Thorns_DoesNotThrow_WhenTakingDamageWithNullSource()
    {
        winner = CreatePlayerWithDeck(3);
        winner.AddStatusEffect(new ThornsEffect(6));
        winner.AddStatusEffect(new BleedingEffect(2));

        Assert.DoesNotThrow(() => winner.DrawOneCard());
        Assert.AreEqual(99f, winner.CurrentHealth);
        Assert.AreEqual(6, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void Thorns_RetaliationDamageIsAbsorbedByAttackerShield()
    {
        winner.AddShield(4f);
        loser.AddStatusEffect(new ThornsEffect(9));

        loser.TakeDamage(new DamageContext
        {
            Source = winner,
            Target = loser,
            Amount = 5f,
            IsRetaliation = false
        });

        Assert.AreEqual(0f, winner.CurrentShield);
        Assert.AreEqual(95f, winner.CurrentHealth);
    }

    [Test]
    public void StatusEffects_SameTypeAccumulatesStacks_InsteadOfDuplicatingEntry()
    {
        winner.AddStatusEffect(new BleedingEffect(3));
        winner.AddStatusEffect(new BleedingEffect(4));

        Assert.AreEqual(1, winner.activeStatusEffects.Count);
        Assert.AreEqual(7, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void StatusEffects_DifferentTypesCoexistIndependently()
    {
        winner.AddStatusEffect(new BleedingEffect(3));
        winner.AddStatusEffect(new ThornsEffect(5));

        Assert.AreEqual(2, winner.activeStatusEffects.Count);
    }

    #endregion

    #region 7. Mazo (Deck), Mano (Hand) y Condiciones de Fin de Partida

    [Test]
    public void Deck_BurnCards_CapsSafelyWhenBurningMoreThanRemainingCards()
    {
        loser = CreatePlayerWithDeck(4);

        Assert.DoesNotThrow(() => loser.Deck.BurnCards(10));
        Assert.AreEqual(0, loser.Deck.CountCards());
        Assert.IsTrue(loser.Deck.IsDeckEmpty());
    }

    [Test]
    public void Deck_BurnCards_OnEmptyDeck_DoesNothingSafely()
    {
        loser = CreatePlayerWithDeck(0);

        Assert.DoesNotThrow(() => loser.Deck.BurnCards(5));
        Assert.AreEqual(0, loser.Deck.CountCards());
    }

    [Test]
    public void PlayerState_DrawAndPlayCard_UpdatesHandDeckAndOwner()
    {
        winner = CreatePlayerWithDeck(3);

        CardStatus drawn = winner.DrawOneCard();
        Assert.IsNotNull(drawn);
        Assert.AreEqual(winner, drawn.owner);
        Assert.AreEqual(1, winner.Hand.Count);
        Assert.AreEqual(2, winner.Deck.CountCards());

        bool played = winner.PlayCard(drawn);
        Assert.IsTrue(played);
        Assert.AreEqual(0, winner.Hand.Count);
        Assert.AreEqual(drawn, winner.PlayedCard);

        winner.ClearPlayedCard();
        Assert.IsNull(winner.PlayedCard);
    }

    [Test]
    public void PlayerState_PlayCard_FailsIfCardIsNotInHand()
    {
        CardStatus foreignCard = new CardStatus(CreateCard(Suit.Swords), winner);

        bool result = winner.PlayCard(foreignCard);

        Assert.IsFalse(result);
        Assert.IsNull(winner.PlayedCard);
    }

    [Test]
    public void PlayerState_HasNoCardsLeft_TrueOnlyWhenHandAndDeckAreBothEmpty()
    {
        winner = CreatePlayerWithDeck(1);
        Assert.IsFalse(winner.HasNoCardsLeft());

        CardStatus lastCard = winner.DrawOneCard();
        Assert.IsFalse(winner.HasNoCardsLeft());

        winner.PlayCard(lastCard);
        Assert.IsTrue(winner.HasNoCardsLeft());
    }

    [Test]
    public void HandComponent_RespectsMaxCardsLimit_AndClearsProperly()
    {
        Hand hand = CreateHandComponent();
        CardStatus c1 = new CardStatus(CreateCard(Suit.Swords), winner);
        CardStatus c2 = new CardStatus(CreateCard(Suit.Clubs), winner);
        CardStatus c3 = new CardStatus(CreateCard(Suit.Cups), winner);
        CardStatus c4 = new CardStatus(CreateCard(Suit.Golds), winner);

        hand.DrawCard(c1);
        hand.DrawCard(c2);
        hand.DrawCard(c3);
        hand.DrawCard(c4);

        Assert.AreEqual(3, hand.cards.Count);
        Assert.IsFalse(hand.cards.Contains(c4));

        hand.ClearHand();
        Assert.AreEqual(0, hand.cards.Count);
    }

    [Test]
    public void GameOver_False_WhenBothPlayersAliveAndHaveCards()
    {
        winner = CreatePlayerWithDeck(5);
        loser = CreatePlayerWithDeck(5);

        Assert.IsFalse(BriscaRules.CheckGameOverConditions(winner, loser));
        Assert.IsNull(BriscaRules.CheckForWinner(winner, loser));
    }

    [Test]
    public void GameOver_True_WhenEitherPlayerHealthReachesZero()
    {
        winner = CreatePlayerWithDeck(5);
        loser = CreatePlayerWithDeck(5);

        loser.TakeDamage(new DamageContext { Amount = 100f });

        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
    }

    [Test]
    public void GameOver_WhenOnlyFirstPlayerRunsOutOfCards_SecondPlayerWins()
    {
        winner = CreatePlayerWithDeck(0);
        loser = CreatePlayerWithDeck(3);

        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
        Assert.AreEqual(loser, BriscaRules.CheckForWinner(winner, loser));
    }

    [Test]
    public void GameOver_WhenOnlySecondPlayerRunsOutOfCards_FirstPlayerWins()
    {
        winner = CreatePlayerWithDeck(4);
        loser = CreatePlayerWithDeck(0);

        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
        Assert.AreEqual(winner, BriscaRules.CheckForWinner(winner, loser));
    }

    [Test]
    public void GameOver_WhenBothPlayersRunOutOfCards_PlayerWithHigherHealthWins()
    {
        winner = CreatePlayerWithDeck(0);
        loser = CreatePlayerWithDeck(0);
        loser.TakeDamage(new DamageContext { Amount = 15f });

        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
        Assert.AreEqual(winner, BriscaRules.CheckForWinner(winner, loser));
    }

    [Test]
    public void GameOver_WhenBothRunOutOfCardsWithEqualHealth_DefaultsToSecondPlayer()
    {
        winner = CreatePlayerWithDeck(0);
        loser = CreatePlayerWithDeck(0);

        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
        Assert.AreEqual(loser, BriscaRules.CheckForWinner(winner, loser));
    }

    #endregion

    #region 8. Integración de Baza Completa (BriscaRules + CombatManager)

    [Test]
    public void FullTrick_EndToEnd_TriumphSwordsVsClubs_ResolvesWinnerAndAppliesBothPowers()
    {
        // Triunfo: Espadas. Jugador 1 juega Rey de Bastos (4), Jugador 2 juega As de Espadas (11)
        PreparePlayedCard(winner, Suit.Clubs, Rank.King);
        PreparePlayedCard(loser, Suit.Swords, Rank.Ace);

        TrickResult trick = BriscaRules.CalculateTrickWinner(Suit.Swords, winner, loser);
        Assert.AreEqual(loser, trick.winner);
        Assert.AreEqual(winner, trick.loser);

        CombatManager.ApplyWinnerPower(trick.winner, trick.loser);
        CombatManager.ApplyLoserPower(trick.loser, trick.winner);

        // Jugador 2 (ganador con As de Espadas) hace 11 de daño a Jugador 1
        Assert.AreEqual(89f, winner.CurrentHealth);
        // Jugador 1 (perdedor con Rey de Bastos) gana 4 stacks de Espinas
        Assert.AreEqual(1, winner.activeStatusEffects.Count);
        Assert.AreEqual(4, winner.activeStatusEffects[0].Stacks);
    }

    [Test]
    public void FullTrick_TwoTurns_GoldsLossInvestsIntoNextSwordsWin()
    {
        // Turno 1: Jugador 1 juega As de Oros (11) pero pierde contra Dos de Bastos (Triunfo -> da 1 de escudo)
        PreparePlayedCard(winner, Suit.Golds, Rank.Ace);
        PreparePlayedCard(loser, Suit.Clubs, Rank.Two);

        TrickResult trick1 = BriscaRules.CalculateTrickWinner(Suit.Clubs, winner, loser);
        CombatManager.ApplyWinnerPower(trick1.winner, trick1.loser);
        CombatManager.ApplyLoserPower(trick1.loser, trick1.winner);

        Assert.AreEqual(11, winner.Investment);
        Assert.AreEqual(1f, loser.CurrentShield);

        // Turno 2: Jugador 1 gana con Tres de Espadas (10 + 11 de Investment = 21 de daño)
        PreparePlayedCard(winner, Suit.Swords, Rank.Three);
        PreparePlayedCard(loser, Suit.Golds, Rank.Two);

        TrickResult trick2 = BriscaRules.CalculateTrickWinner(Suit.Clubs, winner, loser);
        CombatManager.ApplyWinnerPower(trick2.winner, trick2.loser);
        CombatManager.ApplyLoserPower(trick2.loser, trick2.winner);

        // Jugador 2 tenía 1 de escudo: absorbe 1 y recibe 20 de daño -> 80 HP
        Assert.AreEqual(0f, loser.CurrentShield);
        Assert.AreEqual(80f, loser.CurrentHealth);
        Assert.AreEqual(0, winner.Investment);
        Assert.AreEqual(1, loser.Investment);
    }

    [Test]
    public void FullTrick_ThornsRetaliationCanKillAttacker_AndTriggerGameOver()
    {
        winner = CreatePlayerWithDeck(5);
        loser = CreatePlayerWithDeck(5);

        winner.TakeDamage(new DamageContext { Amount = 96f }); // Jugador 1 queda a 4 HP
        loser.AddStatusEffect(new ThornsEffect(5)); // Jugador 2 tiene 5 de Espinas

        // Jugador 1 gana la baza con As de Espadas (11 de daño) y activa las Espinas de Jugador 2 (5 de daño)
        PreparePlayedCard(winner, Suit.Swords, Rank.Ace);
        PreparePlayedCard(loser, Suit.Golds, Rank.Two);

        TrickResult trick = BriscaRules.CalculateTrickWinner(Suit.Swords, winner, loser);
        CombatManager.ApplyWinnerPower(trick.winner, trick.loser);

        Assert.IsTrue(winner.IsDead);
        Assert.IsTrue(BriscaRules.CheckGameOverConditions(winner, loser));
    }

    #endregion
}