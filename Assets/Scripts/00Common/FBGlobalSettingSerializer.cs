using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;

public class FBGlobalSettingSerializer
{
    private static StringOffset ToFBString(FlatBufferBuilder builder, string value)
    {
        if (string.IsNullOrEmpty(value) == false)
        {
            return builder.CreateString(value);
        }

        return builder.CreateString(string.Empty);
    }

    public static void SaveFBGlobalSetting(string dataPath, GlobalSetting dataObj)
    {
        FlatBuffers.FlatBufferBuilder builder = new FlatBuffers.FlatBufferBuilder(1);
        StringOffset defaultHitEffectOffset = ToFBString(builder, dataObj.defaultHitEffect);
        StringOffset defaultProjectileHitEffectOffset = ToFBString(builder, dataObj.defaultProjectileHitEffect);
        StringOffset defualtHitSfxOffset = ToFBString(builder, dataObj.defualtHitSfx);
        StringOffset equipListOffset = ToFBString(builder, dataObj.equipList);
        StringOffset scenePathOffset = ToFBString(builder, dataObj.scenePath);
        StringOffset serverListUrlOffset = ToFBString(builder, dataObj.serverListUrl);
        StringOffset critialDeadEffectOffset = ToFBString(builder, dataObj.critialDeadEffect);
        StringOffset immediateDeadEffectOffset = ToFBString(builder, dataObj.immediateDeadEffect);
        StringOffset normalDeadEffectOffset = ToFBString(builder, dataObj.normalDeadEffect);
        StringOffset pvpDefaultSesstionIDOffset = ToFBString(builder, dataObj.pvpDefaultSesstionID);
        VectorOffset loggerFilterOffset = default(VectorOffset);
        if (null != dataObj.loggerFilter && dataObj.loggerFilter.Length > 0)
        {
            StringOffset[] loggerFilterStringOffset = new StringOffset[dataObj.loggerFilter.Length];
            for (int i = 0, icnt = dataObj.loggerFilter.Length; i < icnt; ++i)
            {
                string logerFilter = dataObj.loggerFilter[i];
                loggerFilterStringOffset[i] = ToFBString(builder, logerFilter);
            }

            loggerFilterOffset = FBGlobalSetting.GlobalSetting.CreateLoggerFilterVector(builder, loggerFilterStringOffset);
        }

        Dictionary<string, float>.Enumerator it = dataObj.equipPropFactors.GetEnumerator();
        float[] equipPropFactorsValueArray = new float[dataObj.equipPropFactors.Count];
        StringOffset[] equipPropFactorsKeyArray = new StringOffset[dataObj.equipPropFactors.Count];
        int unCount = 0;
        while (it.MoveNext())
        {
            KeyValuePair<string, float> cur = it.Current;
            equipPropFactorsValueArray[unCount] = cur.Value;
            equipPropFactorsKeyArray[unCount] = ToFBString(builder, cur.Key);
            ++unCount;
        }
        VectorOffset equipPropFactorsKeyVector = FBGlobalSetting.GlobalSetting.CreateEquipPropFactorsKeyVector(builder, equipPropFactorsKeyArray);
        VectorOffset equipPropFactorsValueVector = FBGlobalSetting.GlobalSetting.CreateEquipPropFactorsValueVector(builder, equipPropFactorsValueArray);

        VectorOffset equipPropFactorValuesVector = FBGlobalSetting.GlobalSetting.CreateEquipPropFactorValuesVector(builder, dataObj.equipPropFactorValues);

        it = dataObj.quipThirdTypeFactors.GetEnumerator();
        float[] quipThirdTypeFactorsValueArray = new float[dataObj.quipThirdTypeFactors.Count];
        StringOffset[] quipThirdTypeFactorsKeyArray = new StringOffset[dataObj.quipThirdTypeFactors.Count];
        unCount = 0;
        while (it.MoveNext())
        {
            KeyValuePair<string, float> cur = it.Current;
            quipThirdTypeFactorsValueArray[unCount] = cur.Value;
            quipThirdTypeFactorsKeyArray[unCount] = ToFBString(builder, cur.Key);
            ++unCount;
        }
        VectorOffset quipThirdTypeFactorsKeyVector = FBGlobalSetting.GlobalSetting.CreateQuipThirdTypeFactorsKeyVector(builder, quipThirdTypeFactorsKeyArray);
        VectorOffset quipThirdTypeFactorsValueVector = FBGlobalSetting.GlobalSetting.CreateQuipThirdTypeFactorsValueVector(builder, quipThirdTypeFactorsValueArray);

        VectorOffset quipThirdTypeFactorValuesVector = FBGlobalSetting.GlobalSetting.CreateQuipThirdTypeFactorValuesVector(builder, dataObj.quipThirdTypeFactorValues);


        VectorOffset addressOff = default(VectorOffset);
        if (dataObj.serverList.Length > 0)
        {
            Offset<FBGlobalSetting.Address>[] serverLstVector = new Offset<FBGlobalSetting.Address>[dataObj.serverList.Length];
            for (int i = 0, icnt = dataObj.serverList.Length; i < icnt; ++i)
            {
                GlobalSetting.Address curAddress = dataObj.serverList[i];

                StringOffset addressNameOff = ToFBString(builder, curAddress.name);
                StringOffset addressIPOff = ToFBString(builder, curAddress.ip);
                serverLstVector[i] = FBGlobalSetting.Address.CreateAddress(
                    builder, addressNameOff, addressIPOff, curAddress.port, curAddress.id);
            }

            addressOff = FBGlobalSetting.GlobalSetting.CreateServerListVector(builder, serverLstVector);
        }
        
        var speedAnchorArrayOffset = FBGlobalSetting.GlobalSetting.AddSpeedAnchorArray(builder, dataObj.speedAnchorArray);
        var gravityRateArrayOffset = FBGlobalSetting.GlobalSetting.AddGravityRateArray(builder, dataObj.gravityRateArray);

        FBGlobalSetting.GlobalSetting.StartGlobalSetting(builder);
        FBGlobalSetting.GlobalSetting.AddIsTestTeam(builder, dataObj.isTestTeam);
        FBGlobalSetting.GlobalSetting.AddIsPaySDKDebug(builder, dataObj.isPaySDKDebug);
        FBGlobalSetting.GlobalSetting.AddSdkChannel(builder, (int)dataObj.sdkChannel);
        FBGlobalSetting.GlobalSetting.AddIsBanShuVersion(builder, dataObj.isBanShuVersion);
        FBGlobalSetting.GlobalSetting.AddIsDebug(builder, dataObj.isDebug);
        FBGlobalSetting.GlobalSetting.AddIsLogRecord(builder, dataObj.isLogRecord);
        FBGlobalSetting.GlobalSetting.AddIsRecordPVP(builder, dataObj.isRecordPVP);
        FBGlobalSetting.GlobalSetting.AddShowDebugBox(builder, dataObj.showDebugBox);
        FBGlobalSetting.GlobalSetting.AddFrameLock(builder, dataObj.frameLock);
        FBGlobalSetting.GlobalSetting.AddFallgroundHitFactor(builder, dataObj.fallgroundHitFactor);
        FBGlobalSetting.GlobalSetting.AddHitTime(builder, dataObj.hitTime);
        FBGlobalSetting.GlobalSetting.AddDeadWhiteTime(builder, dataObj.deadWhiteTime);
        FBGlobalSetting.GlobalSetting.AddDefaultHitEffect(builder, defaultHitEffectOffset);
        FBGlobalSetting.GlobalSetting.AddDefaultProjectileHitEffect(builder, defaultProjectileHitEffectOffset);
        FBGlobalSetting.GlobalSetting.AddDefualtHitSfx(builder, defualtHitSfxOffset);
        FBGlobalSetting.GlobalSetting.Add_walkSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj._walkSpeed.x, dataObj._walkSpeed.y, dataObj._walkSpeed.z));
        FBGlobalSetting.GlobalSetting.Add_runSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj._runSpeed.x, dataObj._runSpeed.y, dataObj._runSpeed.z));

        FBGlobalSetting.GlobalSetting.AddTownWalkSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.townWalkSpeed.x, dataObj.townWalkSpeed.y, dataObj.townWalkSpeed.z));
        FBGlobalSetting.GlobalSetting.AddTownRunSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.townRunSpeed.x, dataObj.townRunSpeed.y, dataObj.townRunSpeed.z));

        FBGlobalSetting.GlobalSetting.AddTownActionSpeed(builder, dataObj.townActionSpeed);
        FBGlobalSetting.GlobalSetting.AddTownPlayerRun(builder, dataObj.townPlayerRun);
        FBGlobalSetting.GlobalSetting.AddMinHurtTime(builder, dataObj.minHurtTime);
        FBGlobalSetting.GlobalSetting.AddMaxHurtTime(builder, dataObj.maxHurtTime);
        FBGlobalSetting.GlobalSetting.AddFrozenPercent(builder, dataObj.frozenPercent);
        FBGlobalSetting.GlobalSetting.AddJumpBackSpeed(builder,
            FBGlobalSetting.Vector2.CreateVector2(builder, dataObj.jumpBackSpeed.x, dataObj.jumpBackSpeed.y));

        FBGlobalSetting.GlobalSetting.AddJumpForce(builder, dataObj.jumpForce);
        FBGlobalSetting.GlobalSetting.AddClickForce(builder, dataObj.clickForce);
        FBGlobalSetting.GlobalSetting.AddSnapDuration(builder, dataObj.snapDuration);
        FBGlobalSetting.GlobalSetting.Add_dunFuTime(builder, dataObj._dunFuTime);
        FBGlobalSetting.GlobalSetting.Add_pvpDunFuTime(builder, dataObj._pvpDunFuTime);
        FBGlobalSetting.GlobalSetting.AddPVPHPScale(builder, dataObj.PVPHPScale);
        FBGlobalSetting.GlobalSetting.AddTestLevel(builder, dataObj.TestLevel);
        FBGlobalSetting.GlobalSetting.AddTestPlayerNum(builder, dataObj.testPlayerNum);
        FBGlobalSetting.GlobalSetting.AddShowBattleInfoPanel(builder, dataObj.showBattleInfoPanel);
        FBGlobalSetting.GlobalSetting.AddDefaultMonsterID(builder, dataObj.defaultMonsterID);
        FBGlobalSetting.GlobalSetting.Add_monsterWalkSpeedFactor(builder, dataObj._monsterWalkSpeedFactor);
        FBGlobalSetting.GlobalSetting.Add_monsterSightFactor(builder, dataObj._monsterSightFactor);
        FBGlobalSetting.GlobalSetting.AddEnableAssetInstPool(builder, dataObj.enableAssetInstPool);
        FBGlobalSetting.GlobalSetting.AddEnemyHasAI(builder, dataObj.enemyHasAI);
        FBGlobalSetting.GlobalSetting.AddIsCreateMonsterLocal(builder, dataObj.isCreateMonsterLocal);
        FBGlobalSetting.GlobalSetting.AddIsGiveEquips(builder, dataObj.isGiveEquips);
        FBGlobalSetting.GlobalSetting.AddEquipList(builder, equipListOffset);
        FBGlobalSetting.GlobalSetting.AddIsGuide(builder, dataObj.isGuide);
        FBGlobalSetting.GlobalSetting.AddDisplayHUD(builder, dataObj.displayHUD);
        FBGlobalSetting.GlobalSetting.AddCloseTeamCondition(builder, dataObj.CloseTeamCondition);
        FBGlobalSetting.GlobalSetting.AddIsLocalDungeon(builder, dataObj.isLocalDungeon);
        FBGlobalSetting.GlobalSetting.AddLocalDungeonID(builder, dataObj.localDungeonID);
        FBGlobalSetting.GlobalSetting.AddRecordResFile(builder, dataObj.recordResFile);
        FBGlobalSetting.GlobalSetting.AddProfileAssetLoad(builder, dataObj.profileAssetLoad);
        FBGlobalSetting.GlobalSetting.Add_gravity(builder, dataObj._gravity);
        FBGlobalSetting.GlobalSetting.Add_fallGravityReduceFactor(builder, dataObj._fallGravityReduceFactor);
        FBGlobalSetting.GlobalSetting.AddSkillHasCooldown(builder, dataObj.skillHasCooldown);
        FBGlobalSetting.GlobalSetting.AddScenePath(builder, scenePathOffset);
        FBGlobalSetting.GlobalSetting.AddIpSelectedIndex(builder, dataObj.ipSelectedIndex);
        FBGlobalSetting.GlobalSetting.AddISingleCharactorID(builder, dataObj.iSingleCharactorID)         ;
        FBGlobalSetting.GlobalSetting.AddCameraInRange(builder,
            FBGlobalSetting.Vector2.CreateVector2(builder, dataObj.cameraInRange.x, dataObj.cameraInRange.y));

        FBGlobalSetting.GlobalSetting.AddButtonType(builder, (int)dataObj.buttonType)                         ;
        FBGlobalSetting.GlobalSetting.Add_defaultFloatHurt(builder, dataObj._defaultFloatHurt);
        FBGlobalSetting.GlobalSetting.Add_defaultFloatLevelHurat(builder, dataObj._defaultFloatLevelHurat);
        FBGlobalSetting.GlobalSetting.Add_defaultGroundHurt(builder, dataObj._defaultGroundHurt);
        FBGlobalSetting.GlobalSetting.Add_defaultStandHurt(builder, dataObj._defaultStandHurt);
        FBGlobalSetting.GlobalSetting.Add_fallProtectGravityAddFactor(builder, dataObj._fallProtectGravityAddFactor);
        FBGlobalSetting.GlobalSetting.Add_protectClearDuration(builder, (int)dataObj._protectClearDuration);
        FBGlobalSetting.GlobalSetting.AddBgmStart(builder, dataObj.bgmStart);
        FBGlobalSetting.GlobalSetting.AddBgmTown(builder, dataObj.bgmTown);
        FBGlobalSetting.GlobalSetting.AddBgmBattle(builder, dataObj.bgmBattle);
        FBGlobalSetting.GlobalSetting.Add_zDimFactor(builder, dataObj._zDimFactor);
        FBGlobalSetting.GlobalSetting.AddBullteScale(builder, dataObj.bullteScale);
        FBGlobalSetting.GlobalSetting.AddBullteTime(builder, dataObj.bullteTime);
        FBGlobalSetting.GlobalSetting.AddStartSystem(builder, (int)dataObj.startSystem);
        FBGlobalSetting.GlobalSetting.AddLoggerFilter(builder, loggerFilterOffset);
        FBGlobalSetting.GlobalSetting.AddShowDialog(builder, dataObj.showDialog);
        FBGlobalSetting.GlobalSetting.AddAvatarLightDir(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.avatarLightDir.x, dataObj.avatarLightDir.y, dataObj.avatarLightDir.z));
        FBGlobalSetting.GlobalSetting.AddShadowLightDir(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.shadowLightDir.x, dataObj.shadowLightDir.y, dataObj.shadowLightDir.z));
        FBGlobalSetting.GlobalSetting.AddStartVel(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.startVel.x, dataObj.startVel.y, dataObj.startVel.z));
        FBGlobalSetting.GlobalSetting.AddEndVel(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.endVel.x, dataObj.endVel.y, dataObj.endVel.z));
        FBGlobalSetting.GlobalSetting.AddOffset(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.offset.x, dataObj.offset.y, dataObj.offset.z));
        FBGlobalSetting.GlobalSetting.AddTimeAccerlate(builder, dataObj.TimeAccerlate)                    ;
        FBGlobalSetting.GlobalSetting.AddTotalTime(builder, dataObj.TotalTime)                              ;
        FBGlobalSetting.GlobalSetting.AddTotalDist(builder, dataObj.TotalDist)                              ;
        FBGlobalSetting.GlobalSetting.AddHeightAdoption(builder, dataObj.heightAdoption)                   ;
        FBGlobalSetting.GlobalSetting.AddDebugDrawBlock(builder, dataObj.debugDrawBlock)                   ;
        FBGlobalSetting.GlobalSetting.AddLoadFromPackage(builder, dataObj.loadFromPackage)                 ;
        FBGlobalSetting.GlobalSetting.AddEnableHotFix(builder, dataObj.enableHotFix)                       ;
        FBGlobalSetting.GlobalSetting.AddHotFixUrlDebug(builder, dataObj.hotFixUrlDebug)                   ;
        FBGlobalSetting.GlobalSetting.AddREVIVESHOCKSKILLID(builder, dataObj.REVIVE_SHOCK_SKILLID)          ;
        FBGlobalSetting.GlobalSetting.AddRollSpeed(builder,
            FBGlobalSetting.Vector2.CreateVector2(builder, dataObj.rollSpeed.x, dataObj.rollSpeed.y));

        FBGlobalSetting.GlobalSetting.AddRollRand(builder, dataObj.rollRand)                              ;
        FBGlobalSetting.GlobalSetting.AddNormalRollRand(builder, dataObj.normalRollRand)                  ;
        FBGlobalSetting.GlobalSetting.Add_pkDamageAdjustFactor(builder, dataObj._pkDamageAdjustFactor)     ;
        FBGlobalSetting.GlobalSetting.Add_pkHPAdjustFactor(builder, dataObj._pkHPAdjustFactor)             ;
        FBGlobalSetting.GlobalSetting.AddPkUseMaxLevel(builder, dataObj.pkUseMaxLevel)                     ;
        FBGlobalSetting.GlobalSetting.AddBattleRunMode(builder, (int)dataObj.battleRunMode)                      ;
        FBGlobalSetting.GlobalSetting.AddHasDoubleRun(builder, dataObj.hasDoubleRun)                       ;
        FBGlobalSetting.GlobalSetting.AddPlayerHP(builder, dataObj.playerHP)                                ;
        FBGlobalSetting.GlobalSetting.AddPlayerRebornHP(builder, dataObj.playerRebornHP)                    ;
        FBGlobalSetting.GlobalSetting.AddMonsterHP(builder, dataObj.monsterHP)                              ;
        FBGlobalSetting.GlobalSetting.AddPlayerPos(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.playerPos.x, dataObj.playerPos.y, dataObj.playerPos.z));
        FBGlobalSetting.GlobalSetting.AddTransportDoorRadius(builder, dataObj.transportDoorRadius)        ;
        FBGlobalSetting.GlobalSetting.AddPetXMovingDis(builder, dataObj.petXMovingDis)                    ;
        FBGlobalSetting.GlobalSetting.AddPetXMovingv1(builder,  dataObj.petXMovingv1)                      ;
        FBGlobalSetting.GlobalSetting.AddPetXMovingv2(builder,  dataObj.petXMovingv2)                      ;
        FBGlobalSetting.GlobalSetting.AddPetYMovingDis(builder, dataObj.petYMovingDis)                    ;
        FBGlobalSetting.GlobalSetting.AddPetYMovingv1(builder,  dataObj.petYMovingv1)                      ;
        FBGlobalSetting.GlobalSetting.AddPetYMovingv2(builder, dataObj.petYMovingv2)                      ;
        FBGlobalSetting.GlobalSetting.AddServerListUrl(builder, serverListUrlOffset)       ;

        FBGlobalSetting.GlobalSetting.AddServerList(builder, addressOff);

        FBGlobalSetting.GlobalSetting.AddDebugNewAutofightAI(builder, dataObj.aiHotReload)         ;
        FBGlobalSetting.GlobalSetting.AddCharScale(builder, dataObj.charScale);
        FBGlobalSetting.GlobalSetting.AddMonsterBeHitShockData(builder,
            FBGlobalSetting.ShockData.CreateShockData(builder, dataObj.monsterBeHitShockData.time, dataObj.monsterBeHitShockData.speed, dataObj.monsterBeHitShockData.xrange, dataObj.monsterBeHitShockData.yrange, dataObj.monsterBeHitShockData.mode));
        FBGlobalSetting.GlobalSetting.AddPlayerBeHitShockData(builder,
            FBGlobalSetting.ShockData.CreateShockData(builder, dataObj.playerBeHitShockData.time, dataObj.playerBeHitShockData.speed, dataObj.playerBeHitShockData.xrange, dataObj.playerBeHitShockData.yrange, dataObj.playerBeHitShockData.mode)); 
        FBGlobalSetting.GlobalSetting.AddPlayerSkillCDShockData(builder,
            FBGlobalSetting.ShockData.CreateShockData(builder, dataObj.playerSkillCDShockData.time, dataObj.playerSkillCDShockData.speed, dataObj.playerSkillCDShockData.xrange, dataObj.playerSkillCDShockData.yrange, dataObj.playerSkillCDShockData.mode));
        FBGlobalSetting.GlobalSetting.AddPlayerHighFallTouchGroundShockData(builder,
            FBGlobalSetting.ShockData.CreateShockData(builder, dataObj.playerHighFallTouchGroundShockData.time, dataObj.playerHighFallTouchGroundShockData.speed, dataObj.playerHighFallTouchGroundShockData.xrange, dataObj.playerHighFallTouchGroundShockData.yrange, dataObj.playerHighFallTouchGroundShockData.mode));
        FBGlobalSetting.GlobalSetting.AddHighFallHight(builder, dataObj.highFallHight)                                                               ;
        FBGlobalSetting.GlobalSetting.AddCritialDeadEffect(builder, critialDeadEffectOffset)                                          ;
        FBGlobalSetting.GlobalSetting.AddImmediateDeadEffect(builder, immediateDeadEffectOffset)                                      ;
        FBGlobalSetting.GlobalSetting.AddNormalDeadEffect(builder, normalDeadEffectOffset)                                            ;
        FBGlobalSetting.GlobalSetting.AddEnableEffectLimit(builder, dataObj.enableEffectLimit)                                                        ;
        FBGlobalSetting.GlobalSetting.AddEffectLimitCount(builder, dataObj.effectLimitCount)                                                           ;
        FBGlobalSetting.GlobalSetting.AddImmediateDeadHPPercent(builder, dataObj.immediateDeadHPPercent)                                               ;
        FBGlobalSetting.GlobalSetting.AddOpenBossShow(builder, dataObj.openBossShow)                                                                  ;
        FBGlobalSetting.GlobalSetting.AddShooterFitPercent(builder, dataObj.shooterFitPercent)                                                       ;
        FBGlobalSetting.GlobalSetting.AddDisappearDis(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj.disappearDis.x, dataObj.disappearDis.y, dataObj.disappearDis.z));
        FBGlobalSetting.GlobalSetting.AddKeepDis(builder, dataObj.keepDis);
        FBGlobalSetting.GlobalSetting.AddAccompanyRunTime(builder, dataObj.accompanyRunTime)                  ;
        FBGlobalSetting.GlobalSetting.Add_aiWanderRange(builder,            dataObj._aiWanderRange)                         ;
        FBGlobalSetting.GlobalSetting.Add_aiWAlkBackRange(builder,          dataObj._aiWAlkBackRange)                     ;
        FBGlobalSetting.GlobalSetting.Add_aiMaxWalkCmdCount(builder,        dataObj._aiMaxWalkCmdCount)                 ;
        FBGlobalSetting.GlobalSetting.Add_aiMaxWalkCmdCountRANGED(builder, dataObj._aiMaxWalkCmdCount_RANGED)     ;
        FBGlobalSetting.GlobalSetting.Add_aiMaxIdleCmdcount(builder, dataObj._aiMaxIdleCmdcount)                 ;
        FBGlobalSetting.GlobalSetting.Add_aiSkillAttackPassive(builder, dataObj._aiSkillAttackPassive)           ;
        FBGlobalSetting.GlobalSetting.Add_monsterGetupBatiFactor(builder, dataObj._monsterGetupBatiFactor)     ;
        FBGlobalSetting.GlobalSetting.Add_degangBackDistance(builder, dataObj._degangBackDistance)             ;
        FBGlobalSetting.GlobalSetting.Add_afThinkTerm(builder, dataObj._afThinkTerm)                             ;
        FBGlobalSetting.GlobalSetting.Add_afFindTargetTerm(builder, dataObj._afFindTargetTerm)                   ;
        FBGlobalSetting.GlobalSetting.Add_afChangeDestinationTerm(builder, dataObj._afChangeDestinationTerm)     ;
        FBGlobalSetting.GlobalSetting.Add_autoCheckRestoreInterval(builder, dataObj._autoCheckRestoreInterval)   ;
        FBGlobalSetting.GlobalSetting.AddForceUseAutoFight(builder, dataObj.forceUseAutoFight)                 ;
        FBGlobalSetting.GlobalSetting.AddCanUseAutoFight(builder, dataObj.canUseAutoFight)                     ;
        FBGlobalSetting.GlobalSetting.AddCanUseAutoFightFirstPass(builder, dataObj.canUseAutoFightFirstPass)   ;
        FBGlobalSetting.GlobalSetting.AddLoadAutoFight(builder, dataObj.loadAutoFight)                         ;
        FBGlobalSetting.GlobalSetting.AddJumpAttackLimitHeight(builder, dataObj.jumpAttackLimitHeight)        ;
        FBGlobalSetting.GlobalSetting.AddSkillCancelLimitTime(builder, dataObj.skillCancelLimitTime)          ;
        FBGlobalSetting.GlobalSetting.AddDoublePressCheckDuration(builder, dataObj.doublePressCheckDuration)    ;
        FBGlobalSetting.GlobalSetting.AddWalkAction(builder, (int)dataObj.walkAction)                                ;
        FBGlobalSetting.GlobalSetting.AddRunAction(builder, (int)dataObj.runAction)                                  ;
        FBGlobalSetting.GlobalSetting.AddWalkAniFactor(builder, dataObj.walkAniFactor)                        ;
        FBGlobalSetting.GlobalSetting.AddRunAniFactor(builder, dataObj.runAniFactor)                          ;
        FBGlobalSetting.GlobalSetting.AddChangeFaceStop(builder, dataObj.changeFaceStop)                       ;
        FBGlobalSetting.GlobalSetting.Add_monsterWalkSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj._monsterWalkSpeed.x, dataObj._monsterWalkSpeed.y, dataObj._monsterWalkSpeed.z));
        FBGlobalSetting.GlobalSetting.Add_monsterRunSpeed(builder,
            FBGlobalSetting.Vector3.CreateVector3(builder, dataObj._monsterRunSpeed.x, dataObj._monsterRunSpeed.y, dataObj._monsterRunSpeed.z));
        FBGlobalSetting.GlobalSetting.AddTableLoadStep(builder, dataObj.tableLoadStep);
        FBGlobalSetting.GlobalSetting.AddLoadingProgressStepInEditor(builder, dataObj.loadingProgressStepInEditor);
        FBGlobalSetting.GlobalSetting.AddLoadingProgressStep(builder, dataObj.loadingProgressStep);
        FBGlobalSetting.GlobalSetting.AddPvpDefaultSesstionID(builder, pvpDefaultSesstionIDOffset);
        FBGlobalSetting.GlobalSetting.AddPetID(builder, dataObj.petID);
        FBGlobalSetting.GlobalSetting.AddPetLevel(builder, dataObj.petLevel);
        FBGlobalSetting.GlobalSetting.AddPetHunger(builder, dataObj.petHunger);
        FBGlobalSetting.GlobalSetting.AddPetSkillIndex(builder, dataObj.petSkillIndex);
        FBGlobalSetting.GlobalSetting.AddTestFashionEquip(builder, dataObj.testFashionEquip);

        FBGlobalSetting.GlobalSetting.AddEquipPropFactorsKey(builder, equipPropFactorsKeyVector);
        FBGlobalSetting.GlobalSetting.AddEquipPropFactorsValue(builder, equipPropFactorsValueVector);

        FBGlobalSetting.GlobalSetting.AddEquipPropFactorValues(builder, equipPropFactorValuesVector);

        FBGlobalSetting.GlobalSetting.AddQuipThirdTypeFactorsKey(builder, quipThirdTypeFactorsKeyVector);
        FBGlobalSetting.GlobalSetting.AddQuipThirdTypeFactorsValue(builder, quipThirdTypeFactorsValueVector);

        FBGlobalSetting.GlobalSetting.AddQuipThirdTypeFactorValues(builder, quipThirdTypeFactorValuesVector);

        FBGlobalSetting.GlobalSetting.AddQualityAdjust(builder,
            FBGlobalSetting.QualityAdjust.CreateQualityAdjust(builder, dataObj.qualityAdjust.bIsOpen, dataObj.qualityAdjust.fInterval, dataObj.qualityAdjust.iTimes));
        FBGlobalSetting.GlobalSetting.AddPetDialogLife(builder, dataObj.petDialogLife);
        FBGlobalSetting.GlobalSetting.AddPetDialogShowInterval(builder, dataObj.petDialogShowInterval);
        FBGlobalSetting.GlobalSetting.AddPetSpecialIdleInterval(builder, dataObj.petSpecialIdleInterval);
        FBGlobalSetting.GlobalSetting.AddNotifyItemTimeLess(builder, dataObj.notifyItemTimeLess);
        FBGlobalSetting.GlobalSetting.AddUseNewHurtAction(builder, dataObj.useNewHurtAction);
        FBGlobalSetting.GlobalSetting.AddUseNewGravity(builder, dataObj.useNewGravity);
        FBGlobalSetting.GlobalSetting.AddSpeedAnchorArrayVector(builder, speedAnchorArrayOffset);
        FBGlobalSetting.GlobalSetting.AddGravityRateArrayVector(builder, gravityRateArrayOffset);

        Offset<FBGlobalSetting.GlobalSetting> globalSettingOffset =
        FBGlobalSetting.GlobalSetting.EndGlobalSetting(builder);

        builder.Finish(globalSettingOffset.Value, "GSET");
        using (var ms = new MemoryStream(builder.DataBuffer.Data, builder.DataBuffer.Position, builder.Offset))
            File.WriteAllBytes(dataPath, ms.ToArray());
    }

    public static void LoadFBGlobalSetting(string dataPath, out GlobalSetting dataSetting)
    {
        dataPath = dataPath.ToLower();

        dataSetting = null;
        if (!File.Exists(dataPath))
            return;

        byte[] newPathData = System.IO.File.ReadAllBytes(dataPath);

#if LOGIC_SERVER
        dataSetting = new GlobalSetting();
#else
        dataSetting = ScriptableObject.CreateInstance<GlobalSetting>();
#endif
        if (null != dataSetting)
        {
            FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(newPathData);
            FBGlobalSetting.GlobalSetting globalSettingdata = FBGlobalSetting.GlobalSetting.GetRootAsGlobalSetting(buffer);
            if (null != globalSettingdata)
            {
                dataSetting.isTestTeam = globalSettingdata.IsTestTeam;
                dataSetting.isPaySDKDebug = globalSettingdata.IsPaySDKDebug;
                dataSetting.sdkChannel = (SDKChannel)globalSettingdata.SdkChannel;
	            dataSetting.isBanShuVersion = globalSettingdata.IsBanShuVersion;
	            dataSetting.isDebug = globalSettingdata.IsDebug ;
	            dataSetting.isLogRecord = globalSettingdata.IsLogRecord;
	            dataSetting.isRecordPVP = globalSettingdata.IsRecordPVP;
                dataSetting.showDebugBox = globalSettingdata.ShowDebugBox;
                dataSetting.frameLock = globalSettingdata.FrameLock;
                dataSetting.fallgroundHitFactor = globalSettingdata.FallgroundHitFactor;
                dataSetting.hitTime = globalSettingdata.HitTime;
                dataSetting.deadWhiteTime = globalSettingdata.DeadWhiteTime;
                dataSetting.defaultHitEffect = globalSettingdata.DefaultHitEffect;
                dataSetting.defaultProjectileHitEffect = globalSettingdata.DefaultProjectileHitEffect;
                dataSetting.defualtHitSfx = globalSettingdata.DefualtHitSfx;
                dataSetting._walkSpeed = new Vec3(globalSettingdata._walkSpeed.X, globalSettingdata._walkSpeed.Y, globalSettingdata._walkSpeed.Z);
                dataSetting._runSpeed = new Vec3(globalSettingdata._runSpeed.X, globalSettingdata._runSpeed.Y, globalSettingdata._runSpeed.Z); //* */

                dataSetting.townWalkSpeed = new Vec3(globalSettingdata.TownWalkSpeed.X, globalSettingdata.TownWalkSpeed.Y, globalSettingdata.TownWalkSpeed.Z); //* */
                dataSetting.townRunSpeed = new Vec3(globalSettingdata.TownRunSpeed.X, globalSettingdata.TownRunSpeed.Y, globalSettingdata.TownRunSpeed.Z); //* */
                dataSetting.townActionSpeed = globalSettingdata.TownActionSpeed;
                dataSetting.townPlayerRun = globalSettingdata.TownPlayerRun;
                dataSetting.minHurtTime = globalSettingdata.MinHurtTime; //* */
	            dataSetting.maxHurtTime = globalSettingdata.MaxHurtTime; //* */
                dataSetting.frozenPercent = globalSettingdata.FrozenPercent; //* */
                dataSetting.jumpBackSpeed = new Vector2(globalSettingdata.JumpBackSpeed.X, globalSettingdata.JumpBackSpeed.Y);//* */
                dataSetting.jumpForce = globalSettingdata.JumpForce; 
                dataSetting.clickForce = globalSettingdata.ClickForce;
                dataSetting.snapDuration = globalSettingdata.SnapDuration;
	            dataSetting._dunFuTime = globalSettingdata._dunFuTime;

                dataSetting._pvpDunFuTime = globalSettingdata._pvpDunFuTime;
	            dataSetting.PVPHPScale = globalSettingdata.PVPHPScale; //* */
                dataSetting.TestLevel = globalSettingdata.TestLevel;
	            dataSetting.testPlayerNum = globalSettingdata.TestPlayerNum;
                dataSetting.showBattleInfoPanel = globalSettingdata.ShowBattleInfoPanel;
	            dataSetting.defaultMonsterID = globalSettingdata.DefaultMonsterID;
	            dataSetting._monsterWalkSpeedFactor = globalSettingdata._monsterWalkSpeedFactor;
	            dataSetting._monsterSightFactor = globalSettingdata._monsterSightFactor;

                dataSetting.enableAssetInstPool = globalSettingdata.EnableAssetInstPool;

                dataSetting.enemyHasAI = globalSettingdata.EnemyHasAI;
                dataSetting.isCreateMonsterLocal = globalSettingdata.IsCreateMonsterLocal;
	            dataSetting.isGiveEquips = globalSettingdata.IsGiveEquips;
	            dataSetting.equipList = globalSettingdata.EquipList;

                dataSetting.isGuide = globalSettingdata.IsGuide;
                //dataSetting.isAnimationInto = globalSettingdata.IsAnimationInto;
                dataSetting.displayHUD = globalSettingdata.DisplayHUD;
                dataSetting.CloseTeamCondition = globalSettingdata.CloseTeamCondition;
                dataSetting.isLocalDungeon = globalSettingdata.IsLocalDungeon;
                dataSetting.localDungeonID = globalSettingdata.LocalDungeonID;

                dataSetting.recordResFile = globalSettingdata.RecordResFile;
                dataSetting.profileAssetLoad = globalSettingdata.ProfileAssetLoad;

                dataSetting._gravity = globalSettingdata._gravity; //* */

                dataSetting._fallGravityReduceFactor = globalSettingdata._fallGravityReduceFactor; //* */

                dataSetting.skillHasCooldown = globalSettingdata.SkillHasCooldown;

                dataSetting.scenePath = globalSettingdata.ScenePath;

                dataSetting.ipSelectedIndex = globalSettingdata.IpSelectedIndex;

                dataSetting.iSingleCharactorID = globalSettingdata.ISingleCharactorID;

                dataSetting.cameraInRange = new Vector2(globalSettingdata.CameraInRange.X, globalSettingdata.CameraInRange.Y);

                dataSetting.buttonType = (InputManager.ButtonMode)globalSettingdata.ButtonType;

                dataSetting._defaultFloatHurt = globalSettingdata._defaultFloatHurt;

                dataSetting._defaultFloatLevelHurat = globalSettingdata._defaultFloatLevelHurat;
                dataSetting._defaultGroundHurt = globalSettingdata._defaultGroundHurt;
                dataSetting._defaultStandHurt = globalSettingdata._defaultStandHurt;
                dataSetting._fallProtectGravityAddFactor = globalSettingdata._fallProtectGravityAddFactor;
                dataSetting._protectClearDuration = globalSettingdata._protectClearDuration;

                dataSetting.bgmStart = globalSettingdata.BgmStart;
                dataSetting.bgmTown = globalSettingdata.BgmTown;
                dataSetting.bgmBattle = globalSettingdata.BgmBattle;

                dataSetting._zDimFactor = globalSettingdata._zDimFactor;              //* */

                dataSetting.bullteScale = globalSettingdata.BullteScale;
                dataSetting.bullteTime = globalSettingdata.BullteTime;

                dataSetting.startSystem = (GameClient.EClientSystem)globalSettingdata.StartSystem;

                string[] loggerFilerArray = new string[globalSettingdata.LoggerFilterLength];
                for (int i = 0,icnt = loggerFilerArray.Length;i<icnt;++i)
                     loggerFilerArray[i] = globalSettingdata.GetLoggerFilter(i);

                dataSetting.loggerFilter = loggerFilerArray;
                dataSetting.showDialog = globalSettingdata.ShowDialog;

                dataSetting.avatarLightDir = new Vector3(globalSettingdata.AvatarLightDir.X, globalSettingdata.AvatarLightDir.Y, globalSettingdata.AvatarLightDir.Z);
                dataSetting.shadowLightDir = new Vector3(globalSettingdata.ShadowLightDir.X, globalSettingdata.ShadowLightDir.Y, globalSettingdata.ShadowLightDir.Z);

                dataSetting.startVel = new Vector3(globalSettingdata.StartVel.X, globalSettingdata.StartVel.Y, globalSettingdata.StartVel.Z);
                dataSetting.endVel = new Vector3(globalSettingdata.EndVel.X, globalSettingdata.EndVel.Y, globalSettingdata.EndVel.Z);
                dataSetting.offset = new Vector3(globalSettingdata.Offset.X, globalSettingdata.Offset.Y, globalSettingdata.Offset.Z);
                dataSetting.TimeAccerlate = globalSettingdata.TimeAccerlate;
	            dataSetting.TotalTime = globalSettingdata.TotalTime;
	            dataSetting.TotalDist = globalSettingdata.TotalDist;

                dataSetting.heightAdoption = globalSettingdata.HeightAdoption;
                dataSetting.debugDrawBlock = globalSettingdata.DebugDrawBlock;
                dataSetting.loadFromPackage = globalSettingdata.LoadFromPackage;
                dataSetting.enableHotFix = globalSettingdata.EnableHotFix;
                dataSetting.hotFixUrlDebug = globalSettingdata.HotFixUrlDebug;

                dataSetting.REVIVE_SHOCK_SKILLID = globalSettingdata.REVIVESHOCKSKILLID;

	            dataSetting.rollSpeed = new Vector2(globalSettingdata.RollSpeed.X, globalSettingdata.RollSpeed.Y);
	            dataSetting.rollRand = globalSettingdata.RollRand;
                dataSetting.normalRollRand = globalSettingdata.NormalRollRand;

	            //PVP天平
	            dataSetting._pkDamageAdjustFactor = globalSettingdata._pkDamageAdjustFactor;
	            dataSetting._pkHPAdjustFactor = globalSettingdata._pkHPAdjustFactor;
	            dataSetting.pkUseMaxLevel = globalSettingdata.PkUseMaxLevel;
	            dataSetting.battleRunMode = (BattleRunMode)globalSettingdata.BattleRunMode;
	            dataSetting.hasDoubleRun = globalSettingdata.HasDoubleRun;
	            dataSetting.playerHP = globalSettingdata.PlayerHP;
	            dataSetting.playerRebornHP = globalSettingdata.PlayerRebornHP;
	            dataSetting.monsterHP = globalSettingdata.MonsterHP;
	            dataSetting.playerPos = new Vec3(globalSettingdata.PlayerPos.X, globalSettingdata.PlayerPos.Y, globalSettingdata.PlayerPos.Z);
	            dataSetting.transportDoorRadius = globalSettingdata.TransportDoorRadius;//传送门半径
	            dataSetting.petXMovingDis = globalSettingdata.PetXMovingDis;
	            dataSetting.petXMovingv1 = globalSettingdata.PetXMovingv1;
	            dataSetting.petXMovingv2 = globalSettingdata.PetXMovingv2;//max
	            dataSetting.petYMovingDis = globalSettingdata.PetYMovingDis;
	            dataSetting.petYMovingv1 = globalSettingdata.PetYMovingv1;
	            dataSetting.petYMovingv2 = globalSettingdata.PetYMovingv2;//max

                dataSetting.serverListUrl = globalSettingdata.ServerListUrl;

                GlobalSetting.Address[] serverListArray = new GlobalSetting.Address[globalSettingdata.ServerListLength];
                for (int i = 0, icnt = serverListArray.Length; i < icnt; ++i)
                {
                    GlobalSetting.Address newAddress = new GlobalSetting.Address();

                    FBGlobalSetting.Address curServerLst = globalSettingdata.GetServerList(i);
                    newAddress.id = curServerLst.Id;
                    newAddress.port = curServerLst.Port;
                    newAddress.name = curServerLst.Name;
                    newAddress.ip = curServerLst.Ip;

                    serverListArray[i] = newAddress;
                }

                dataSetting.serverList = serverListArray;

                //战斗细节相关
                dataSetting.aiHotReload = globalSettingdata.DebugNewAutofightAI;
                dataSetting.charScale = globalSettingdata.CharScale;

	            dataSetting.monsterBeHitShockData = new ShockData(globalSettingdata.MonsterBeHitShockData.Time, globalSettingdata.MonsterBeHitShockData.Speed, globalSettingdata.MonsterBeHitShockData.Xrange, globalSettingdata.MonsterBeHitShockData.Yrange, globalSettingdata.MonsterBeHitShockData.Mode);
	            dataSetting.playerBeHitShockData = new ShockData(globalSettingdata.PlayerBeHitShockData.Time, globalSettingdata.PlayerBeHitShockData.Speed, globalSettingdata.PlayerBeHitShockData.Xrange, globalSettingdata.PlayerBeHitShockData.Yrange, globalSettingdata.PlayerBeHitShockData.Mode);
	            dataSetting.playerSkillCDShockData = new ShockData(globalSettingdata.PlayerSkillCDShockData.Time, globalSettingdata.PlayerSkillCDShockData.Speed, globalSettingdata.PlayerSkillCDShockData.Xrange, globalSettingdata.PlayerSkillCDShockData.Yrange, globalSettingdata.PlayerSkillCDShockData.Mode);
                //从高掉落到地上会震动
                dataSetting.playerHighFallTouchGroundShockData = new ShockData(globalSettingdata.PlayerHighFallTouchGroundShockData.Time, globalSettingdata.PlayerHighFallTouchGroundShockData.Speed, globalSettingdata.PlayerHighFallTouchGroundShockData.Xrange, globalSettingdata.PlayerHighFallTouchGroundShockData.Yrange, globalSettingdata.PlayerHighFallTouchGroundShockData.Mode);
                dataSetting.highFallHight = globalSettingdata.HighFallHight;

	            dataSetting.critialDeadEffect = globalSettingdata.CritialDeadEffect;
	            dataSetting.immediateDeadEffect = globalSettingdata.ImmediateDeadEffect;
                dataSetting.normalDeadEffect = globalSettingdata.NormalDeadEffect;

                dataSetting.enableEffectLimit = globalSettingdata.EnableEffectLimit;
                dataSetting.effectLimitCount = globalSettingdata.EffectLimitCount;

                dataSetting.immediateDeadHPPercent = globalSettingdata.ImmediateDeadHPPercent;
                //废弃
                dataSetting.openBossShow = globalSettingdata.OpenBossShow;
                dataSetting.shooterFitPercent = globalSettingdata.ShooterFitPercent;

	            //随从
	            dataSetting.disappearDis = new Vector3(globalSettingdata.DisappearDis.X, globalSettingdata.DisappearDis.Y, globalSettingdata.DisappearDis.Z);
	            dataSetting.keepDis = globalSettingdata.KeepDis;
	            dataSetting.accompanyRunTime = globalSettingdata.AccompanyRunTime;

	            //怪物AI
	            dataSetting._aiWanderRange = globalSettingdata._aiWanderRange;
                dataSetting._aiWAlkBackRange = globalSettingdata._aiWAlkBackRange;
	            dataSetting._aiMaxWalkCmdCount = globalSettingdata._aiMaxWalkCmdCount;
	            dataSetting._aiMaxWalkCmdCount_RANGED = globalSettingdata._aiMaxWalkCmdCountRANGED;
	            dataSetting._aiMaxIdleCmdcount = globalSettingdata._aiMaxIdleCmdcount; 
	            dataSetting._aiSkillAttackPassive = globalSettingdata._aiSkillAttackPassive;       //* */	
	            dataSetting._monsterGetupBatiFactor = globalSettingdata._monsterGetupBatiFactor;  //* */
	            dataSetting._degangBackDistance = globalSettingdata._degangBackDistance;     //* */

	            //自动战斗
	            dataSetting._afThinkTerm = globalSettingdata._afThinkTerm;
	            dataSetting._afFindTargetTerm = globalSettingdata._afFindTargetTerm;	
	            dataSetting._afChangeDestinationTerm = globalSettingdata._afChangeDestinationTerm;
	            dataSetting._autoCheckRestoreInterval = globalSettingdata._autoCheckRestoreInterval;
	            dataSetting.forceUseAutoFight = globalSettingdata.ForceUseAutoFight;
	            dataSetting.canUseAutoFight = globalSettingdata.CanUseAutoFight;
	            dataSetting.canUseAutoFightFirstPass = globalSettingdata.CanUseAutoFightFirstPass;
	            dataSetting.loadAutoFight = globalSettingdata.LoadAutoFight;

                //跳攻的最低高度设置
                dataSetting.jumpAttackLimitHeight = globalSettingdata.JumpAttackLimitHeight;

                //技能中断时间
                dataSetting.skillCancelLimitTime = globalSettingdata.SkillCancelLimitTime;

                //摇杆配置
                dataSetting.doublePressCheckDuration = globalSettingdata.DoublePressCheckDuration;
                dataSetting.walkAction = (ActionType)globalSettingdata.WalkAction;
                dataSetting.runAction = (ActionType)globalSettingdata.RunAction;
	            dataSetting.walkAniFactor = globalSettingdata.WalkAniFactor;
	            dataSetting.runAniFactor = globalSettingdata.RunAniFactor;
	            dataSetting.changeFaceStop = globalSettingdata.ChangeFaceStop;

	            dataSetting._monsterWalkSpeed = new Vec3(globalSettingdata._monsterWalkSpeed.X, globalSettingdata._monsterWalkSpeed.Y, globalSettingdata._monsterWalkSpeed.Z);//* */
	            dataSetting._monsterRunSpeed = new Vec3(globalSettingdata._monsterRunSpeed.X, globalSettingdata._monsterRunSpeed.Y, globalSettingdata._monsterRunSpeed.Z);//* */

	            dataSetting.tableLoadStep = globalSettingdata.TableLoadStep;
	            dataSetting.loadingProgressStepInEditor = globalSettingdata.LoadingProgressStepInEditor;
	            dataSetting.loadingProgressStep = globalSettingdata.LoadingProgressStep;

                //pvp录像
                dataSetting.pvpDefaultSesstionID = globalSettingdata.PvpDefaultSesstionID;

	            //宠物测试
	            dataSetting.petID = globalSettingdata.PetID;
	            dataSetting.petLevel = globalSettingdata.PetLevel;
	            dataSetting.petHunger = globalSettingdata.PetHunger;
	            dataSetting.petSkillIndex = globalSettingdata.PetSkillIndex;
                //测试
                dataSetting.testFashionEquip = globalSettingdata.TestFashionEquip;

                dataSetting.equipPropFactors = new Dictionary<string, float>(){};

                int total = globalSettingdata.EquipPropFactorsKeyLength < globalSettingdata.EquipPropFactorsValueLength ? globalSettingdata.EquipPropFactorsKeyLength : globalSettingdata.EquipPropFactorsValueLength;
                for (int i = 0,icnt = total; i<icnt;++i)
                    dataSetting.equipPropFactors.Add(globalSettingdata.GetEquipPropFactorsKey(i), globalSettingdata.GetEquipPropFactorsValue(i));

                float[] equipPropFactorValuesArray = new float[globalSettingdata.EquipPropFactorValuesLength];
                for (int i = 0, icnt = equipPropFactorValuesArray.Length; i < icnt; ++i)
                    equipPropFactorValuesArray[i] = globalSettingdata.GetEquipPropFactorValues(i);
                dataSetting.equipPropFactorValues = equipPropFactorValuesArray;

                total = globalSettingdata.QuipThirdTypeFactorsKeyLength < globalSettingdata.QuipThirdTypeFactorsValueLength ? globalSettingdata.QuipThirdTypeFactorsKeyLength : globalSettingdata.QuipThirdTypeFactorsValueLength;
                for (int i = 0, icnt = total; i < icnt; ++i)
                    dataSetting.quipThirdTypeFactors.Add(globalSettingdata.GetQuipThirdTypeFactorsKey(i), globalSettingdata.GetQuipThirdTypeFactorsValue(i));

                float[] quipThirdTypeFactorValuesArray = new float[globalSettingdata.QuipThirdTypeFactorValuesLength];
                for (int i = 0, icnt = quipThirdTypeFactorValuesArray.Length; i < icnt; ++i)
                    quipThirdTypeFactorValuesArray[i] = globalSettingdata.GetQuipThirdTypeFactorValues(i);
                dataSetting.quipThirdTypeFactorValues = quipThirdTypeFactorValuesArray;

                dataSetting.qualityAdjust = new GlobalSetting.QualityAdjust();
                dataSetting.qualityAdjust.fInterval = globalSettingdata.QualityAdjust.FInterval;
                dataSetting.qualityAdjust.bIsOpen = globalSettingdata.QualityAdjust.BIsOpen;
                dataSetting.qualityAdjust.iTimes = globalSettingdata.QualityAdjust.ITimes;

                dataSetting.petDialogLife = globalSettingdata.PetDialogLife;
                dataSetting.petDialogShowInterval = globalSettingdata.PetDialogShowInterval;
                dataSetting.petSpecialIdleInterval = globalSettingdata.PetSpecialIdleInterval;
                dataSetting.notifyItemTimeLess = globalSettingdata.NotifyItemTimeLess;
                dataSetting.useNewHurtAction = globalSettingdata.UseNewHurtAction;
                dataSetting.useNewGravity = globalSettingdata.UseNewGravity;

                int[] speedAnchorArray = new int[globalSettingdata.SpeedAnchorArrayLength];
                for (int i = 0; i < speedAnchorArray.Length; i++)
                    speedAnchorArray[i] = globalSettingdata.SpeedAnchorArray(i);
                dataSetting.speedAnchorArray = speedAnchorArray;

                int[] gravityRateArray = new int[globalSettingdata.GravityRateArrayLength];
                for (int i = 0; i < gravityRateArray.Length; i++)
                    gravityRateArray[i] = globalSettingdata.GravityRateArray(i);
                dataSetting.gravityRateArray = gravityRateArray;
            }
        }
    }
}
