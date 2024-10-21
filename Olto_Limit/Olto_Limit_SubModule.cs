using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace Olto_Limit
{
    public class Olto_Limit_SubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            gameStarterObject.AddModel((GameModel)new OltoPartySizeLimitModel());
            //PartyPrisonerSizeLimit: GetCurrentPartySizeEffect //return party.NumberOfHealthyMembers / 2; //return party.NumberOfHealthyMembers + party.NumberOfWoundedTotalMembers / 2;
        }
    }

    internal class OltoPartySizeLimitModel : DefaultPartySizeLimitModel
    {
        private readonly TextObject _baseSizeText = GameTexts.FindText("str_base_size");
        private readonly TextObject _wallLevelBonusText = GameTexts.FindText("str_map_tooltip_wall_level");
        private readonly TextObject _currentPartySizeBonusText = GameTexts.FindText("str_current_party_size_bonus");
        private static bool _addAdditionalPrisonerSizeAsCheat;

        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false)
        {
            if (party.IsSettlement)
            {
                return CalculateSettlementPartyPrisonerSizeLimitInternal(party.Settlement, includeDescriptions);
            }
            return CalculateMobilePartyPrisonerSizeLimitInternal(party, includeDescriptions);
        }

        private ExplainedNumber CalculateSettlementPartyPrisonerSizeLimitInternal(Settlement settlement, bool includeDescriptions = false)
        {
            ExplainedNumber result = new ExplainedNumber(60f, includeDescriptions, _baseSizeText);
            int num = settlement.Town?.GetWallLevel() ?? 0;
            if (num > 0)
            {
                result.Add(num * 40, _wallLevelBonusText);
            }
            return result;
        }

        private ExplainedNumber CalculateMobilePartyPrisonerSizeLimitInternal(PartyBase party, bool includeDescriptions = false)
        {
            ExplainedNumber result = new ExplainedNumber(10f, includeDescriptions, _baseSizeText);
            result.Add(GetCurrentPartySizeEffect(party), _currentPartySizeBonusText);
            AddMobilePartyLeaderPrisonerSizePerkEffects(party, ref result);
            if (_addAdditionalPrisonerSizeAsCheat && party.IsMobile && party.MobileParty.IsMainParty && Game.Current.CheatMode)
            {
                result.Add(5000f, new TextObject("{=!}Additional size from extra prisoner cheat"));
            }

            return result;
        }

        private void AddMobilePartyLeaderPrisonerSizePerkEffects(PartyBase party, ref ExplainedNumber result)
        {
            if (party.LeaderHero != null)
            {
                if (party.LeaderHero.GetPerkValue(DefaultPerks.TwoHanded.Terror))
                {
                    result.Add(DefaultPerks.TwoHanded.Terror.SecondaryBonus, DefaultPerks.TwoHanded.Terror.Name);
                }

                if (party.LeaderHero.GetPerkValue(DefaultPerks.Athletics.Stamina))
                {
                    result.Add(DefaultPerks.Athletics.Stamina.SecondaryBonus, DefaultPerks.Athletics.Stamina.Name);
                }

                if (party.LeaderHero.GetPerkValue(DefaultPerks.Roguery.Manhunter))
                {
                    result.Add(DefaultPerks.Roguery.Manhunter.SecondaryBonus, DefaultPerks.Roguery.Manhunter.Name);
                }

                if (party.LeaderHero != null && party.LeaderHero.GetPerkValue(DefaultPerks.Scouting.VantagePoint))
                {
                    result.Add(DefaultPerks.Scouting.VantagePoint.SecondaryBonus, DefaultPerks.Scouting.VantagePoint.Name);
                }
            }
        }
//mod
        private int GetCurrentPartySizeEffect(PartyBase party)
        {
//          return party.NumberOfHealthyMembers / 2;
            return party.NumberOfHealthyMembers + party.NumberOfWoundedTotalMembers / 2;
        }

    }
}