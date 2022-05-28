IF OBJECT_ID('tempdb..#tmpQuestionCount') IS NOT NULL
    DROP TABLE #tmpQuestionCount
IF OBJECT_ID('tempdb..#tmpReponsesCount') IS NOT NULL
    DROP TABLE #tmpReponsesCount
IF OBJECT_ID('tempdb..#SubSectionTypes') IS NOT NULL
    DROP TABLE #SubSectionTypes
IF OBJECT_ID('tempdb..#tmpPlots') IS NOT NULL
    DROP TABLE #tmpPlots

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

SELECT * FROM #tmpReponsesCount ORDER BY PlotID DESC
		  
--select PlotID
--,BQCSectionID
--,SubSectionTypeEnum = MAX(#tmpReponsesCount.SubSectionTypeEnumUnformatted)
--INTO #SubSectionTypes
--FROM #tmpReponsesCount
--GROUP BY PlotID
--,BQCSectionID

--SELECT * FROM #SubSectionTypes

SELECT Croudace.dbo.SitePlots.PlotID
INTO #tmpPlots
FROM Croudace.dbo.SitePlots
WHERE Croudace.dbo.SitePlots.SiteID = 438

--SELECT 
--BQCSubSections.BQCSectionID
--,MAX(BQCSubSections.SubSectionTypeEnum) as [SubSectionTypeEnum]
--INTO #tmpSubSectionCategoryEnums 
--FROM BQCSubSections
--WHERE (BQCSubSections.SubSectionTypeEnum <> 3)
--GROUP BY BQCSubSections.BQCSectionID

--SELECT * FROM #tmpSubSectionCategoryEnums

SELECT #tmpPlots.PlotID
,#tmpQuestionCount.BQCSectionID
,IsComplete = IIF(#tmpQuestionCount.QuestionsCount = #tmpReponsesCount.ReponsesCount,1,0)
,QuestionsCount = #tmpQuestionCount.QuestionsCount
,ReponsesCount = SUM(ISNULL(#tmpReponsesCount.ReponsesCount,0))
,BuildStagesCompleted = (SELECT TOP 1 Croudace.dbo.SitePlotBuildStage.StageNo FROM Croudace.dbo.SitePlotBuildStage WHERE Croudace.dbo.SitePlotBuildStage.SitePlotID = #tmpPlots.PlotID ORDER BY Croudace.dbo.SitePlotBuildStage.StageNo DESC)
FROM #tmpQuestionCount
INNER JOIN #tmpReponsesCount ON #tmpQuestionCount.BQCSectionID = #tmpReponsesCount.BQCSectionID
--LEFT JOIN #SubSectionTypes ON #SubSectionTypes.BQCSectionID = #tmpQuestionCount.BQCSectionID
--LEFT JOIN #tmpReponsesCount ON #tmpReponsesCount.BQCSectionID = #SubSectionTypes.BQCSectionID
RIGHT JOIN #tmpPlots ON #tmpPlots.PlotID = #tmpReponsesCount.PlotID
--LEFT JOIN #tmpSubSectionCategoryEnums ON #tmpSubSectionCategoryEnums.BQCSectionID = #tmpQuestionCount.BQCSectionID
GROUP BY #tmpPlots.PlotID
,#tmpQuestionCount.BQCSectionID
,#tmpQuestionCount.QuestionsCount
,#tmpQuestionCount.BQCSectionID
,#tmpReponsesCount.ReponsesCount