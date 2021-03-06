IF OBJECT_ID('tempdb..#tmpQuestionCount') IS NOT NULL
    DROP TABLE #tmpQuestionCount
IF OBJECT_ID('tempdb..#tmpReponsesCount') IS NOT NULL
    DROP TABLE #tmpReponsesCount
IF OBJECT_ID('tempdb..#tmpPlots') IS NOT NULL
    DROP TABLE #tmpPlots
IF OBJECT_ID('tempdb..#tmpAllPlotInformation') IS NOT NULL
    DROP TABLE #tmpAllPlotInformation

;WITH withQuestionsCount AS (
	SELECT 
		BQCSubSections.BQCSubSectionID
		,BQCSections.BQCSectionID
		,QuestionsCount = COUNT(BQCQuestions.BQCQuestionID)
		,BQCSubSections.SubSectionTypeEnum as [SubSectionTypeEnumUnformatted]
		,MAX(BQCSubSections.SubSectionTypeEnum) OVER(PARTITION BY BQCSubSections.BQCSectionID) AS [SubSectionTypeEnumMax]
	FROM 
		BQCQuestions
		LEFT JOIN BQCSubSections ON BQCSubSections.BQCSubSectionID = BQCQuestions.BQCSubSectionID
		LEFT JOIN BQCSections ON BQCSections.BQCSectionID = BQCSubSections.BQCSectionID
	WHERE (1=1)
	AND BQCQuestions.Active = 1
	GROUP BY 
		BQCSubSections.BQCSubSectionID
		,BQCSections.BQCSectionID
		,BQCSubSections.SubSectionTypeEnum
		,BQCSubSections.BQCSectionID
)
SELECT z.*
INTO #tmpQuestionCount
FROM withQuestionsCount z
WHERE [SubSectionTypeEnumUnformatted] = [SubSectionTypeEnumMax]

;WITH withReponsesCount AS (
	SELECT 
		Croudace.dbo.SitePlots.PlotID AS [PlotId]
		,BQCSubSections.BQCSectionID 
		,BQCSubSections.SubSectionTypeEnum as [SubSectionTypeEnumUnformatted]
		,MAX(BQCSubSections.SubSectionTypeEnum) OVER(PARTITION BY BQCSubSections.BQCSectionID) AS [SubSectionTypeEnumMax]
		,ReponsesCount = ISNULL(SUM(CASE WHEN BQCSubSections.SubSectionTypeEnum = 1 THEN AcceptorWorkCompleted ELSE CASE WHEN BQCSubSections.SubSectionTypeEnum = 2 THEN CASE WHEN BQCResponses.ApproverResponseEnumValue IN (1,2) THEN 1 ELSE 0 END END END) ,0)
	FROM 
		BQCResponses
		LEFT JOIN BQCQuestions ON BQCQuestions.BQCQuestionID = BQCResponses.BQCQuestionID
		LEFT JOIN BQCSubSections ON BQCSubSections.BQCSubSectionID = BQCQuestions.BQCSubSectionID
		LEFT JOIN Croudace.dbo.SitePlots ON Croudace.dbo.SitePlots.PlotID = BQCResponses.PlotID
	WHERE (1=1)
	AND (BQCSubSections.SubSectionTypeEnum <> 3) -- Payment Type
	AND (Croudace.dbo.SitePlots.SiteID = 438)
	GROUP BY  BQCSubSections.BQCSectionID 
			,BQCSubSections.SubSectionTypeEnum
			,Croudace.dbo.SitePlots.PlotID
)
SELECT x.PlotId, x.BQCSectionID, x.ReponsesCount, x.SubSectionTypeEnumUnformatted
INTO #tmpReponsesCount
FROM withReponsesCount x
WHERE [SubSectionTypeEnumUnformatted] = [SubSectionTypeEnumMax] ORDER BY [PlotId]

SELECT Croudace.dbo.SitePlots.PlotID
,Croudace.dbo.SitePlots.PlotNo
,HouseTypes.dbo.PlotHouseTypes.HouseTypeID AS HouseTypeID
INTO #tmpPlots
FROM Croudace.dbo.SitePlots
INNER JOIN HouseTypes.dbo.PlotHouseTypes ON HouseTypes.dbo.PlotHouseTypes.HouseTypeID = Croudace.dbo.SitePlots.HouseTypeID
WHERE Croudace.dbo.SitePlots.SiteID = 438

SELECT #tmpPlots.PlotID
,#tmpPlots.PlotNo
,#tmpQuestionCount.BQCSectionID
,IsComplete = IIF(#tmpQuestionCount.QuestionsCount = #tmpReponsesCount.ReponsesCount,1,0)
,HouseTypes.dbo.SiteBaseHouseTypes.Storeys
,COUNT(BQCSections.BQCSectionID) AS TotalSectionCount
,BuildStagesCompleted = (SELECT TOP 1 Croudace.dbo.SitePlotBuildStage.StageNo FROM Croudace.dbo.SitePlotBuildStage WHERE Croudace.dbo.SitePlotBuildStage.SitePlotID = #tmpPlots.PlotID ORDER BY Croudace.dbo.SitePlotBuildStage.StageNo DESC)
INTO #tmpAllPlotInformation
FROM #tmpQuestionCount
INNER JOIN #tmpReponsesCount ON #tmpQuestionCount.BQCSectionID = #tmpReponsesCount.BQCSectionID
RIGHT JOIN #tmpPlots ON #tmpPlots.PlotID = #tmpReponsesCount.PlotID
INNER JOIN HouseTypes.dbo.PlotHouseTypes ON HouseTypes.dbo.PlotHouseTypes.HouseTypeID = #tmpPlots.HouseTypeID
INNER JOIN HouseTypes.dbo.SiteBaseHouseTypes ON HouseTypes.dbo.SiteBaseHouseTypes.BaseTypeID = HouseTypes.dbo.PlotHouseTypes.BaseTypeID
INNER JOIN BQCSections ON BQCSections.ApplicableToStoreysGreaterThanOrEqualTo <= HouseTypes.dbo.SiteBaseHouseTypes.Storeys
GROUP BY #tmpPlots.PlotID
,#tmpPlots.PlotNo
,#tmpQuestionCount.BQCSectionID
,#tmpQuestionCount.QuestionsCount
,#tmpQuestionCount.BQCSectionID
,#tmpReponsesCount.ReponsesCount
,HouseTypes.dbo.SiteBaseHouseTypes.Storeys

SELECT 
#tmpAllPlotInformation.PlotID
,PlotNo AS [PlotNumber]
,CAST(SUM(IsComplete) AS varchar) + '/' + CAST(#tmpAllPlotInformation.TotalSectionCount AS VARCHAR) AS [BuildSectionsCompleted] 
,CAST(BuildStagesCompleted AS varchar) + '/' + CAST(32 AS varchar) as [BuildStagesCompleted]
,IIF(SUM(IsComplete) = #tmpAllPlotInformation.TotalSectionCount,2,1) AS [Status] -- 1 = not complete, 2 = complete
FROM
#tmpAllPlotInformation
GROUP BY 
#tmpAllPlotInformation.PlotID
,PlotNo
,BuildStagesCompleted
,#tmpAllPlotInformation.TotalSectionCount