USE [dbCFGApp]
GO

/****** Object:  StoredProcedure [dbo].[usp_GetCompetitorBenchmarkingFeatureMonthWise]    Script Date: 1/16/2024 8:06:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

---------------------FEATURE--------

--select * from tblProductCFGVisitAction where Attribute = 'Feature'
--select * from tblFeature
--exec [usp_GetCompetitorBenchmarkingFeatureMY] 'ALL','ALL',''
CREATE PROCEDURE [dbo].[usp_GetCompetitorBenchmarkingFeatureMonthWise]
 -- 	@Fyear NVARCHAR(50),
	--@Status NVARCHAR(50),
	--@Quarter nvarchar(50)=''
AS
BEGIN

DECLARE @Q1 INT = 0, @Q2 INT = 0, @Q3 INT = 0, @Q4 INT = 0, @cfgId varchar(max);

set @cfgId = (select STRING_AGG(CFGId, ',')  from tblCFGData where  DelFlag = 0)
--select @cfgId

	select ROW_NUMBER ( )   
    OVER ( PARTITION BY Attribute, SubAttribute,SwarajModel  order by id DESC) as Rn,id
	into #tempBenchmarking from tblProductCFGVisitAction
	order by Id

DECLARE @query1 TABLE (
        ModelName NVARCHAR(255),
        PowerSteering NVARCHAR(1),
        IPTOClutch NVARCHAR(1),
        Gear NVARCHAR(1),
        ShuttleGear NVARCHAR(1),
        OthersDD NVARCHAR(1),
		DCVAvailablity NVARCHAR(1),
		DigitalInstrumentCluster NVARCHAR(1),
		MultispeedPTO NVARCHAR(1),
		[4WD] NVARCHAR(1),
		HighLiftCapacity NVARCHAR(1),
        PowerSteeringCount INT,
        IPTOClutchCount INT,
        GearCount INT,
        ShuttleGearCount INT,
        OthersDDCount INT,
		DCVAvailablityCount INT,
		DigitalInstrumentClusterCount INT,
		MultispeedPTOCount INT,
		[4WDCount] INT,
		HighLiftCapacityCount INT,
        Fyear nvarchar(50),
        --status nvarchar(50),
		State nvarchar(50),
		DateFrom Date
--		Subattribute nvarchar(50)
		    );
   INSERT INTO @query1
            SELECT
                CASE
                    WHEN m.ModelName IN ('717', '717ES NT', '717ES') THEN '717'
                    WHEN m.ModelName IN ('733', '735 FE', '735 FEE', '735 XM', '735FEER') THEN '735'
                    WHEN m.ModelName IN ('742 XT', '744 FE', '744 XM') THEN '744'
                    WHEN m.ModelName IN ('841 XM', '843 XM', '843 XM -OSM') THEN '843'
                    ELSE m.ModelName
                END AS ModelName,
        MAX(PowerSteering) AS PowerSteering,
        MAX(IPTOClutch) AS IPTOClutch,
        MAX(Gear) AS Gear,
        MAX(ShuttleGear) AS ShuttleGear,
        MAX(OthersDD) AS OthersDD,
        MAX(DCVAvailablity) AS DCVAvailablity,
        MAX(DigitalInstrumentCluster) AS DigitalInstrumentCluster,
        MAX(MultispeedPTO) AS MultispeedPTO,
        MAX([4WD]) AS [4WD],
        MAX(HighLiftCapacity) AS HighLiftCapacity,
                SUM(CASE WHEN PowerSteering = 'C' THEN 1 ELSE 0 END) AS PowerSteeringCount,
                SUM(CASE WHEN IPTOClutch = 'C' THEN 1 ELSE 0 END) AS IPTOClutchCount,
                SUM(CASE WHEN Gear = 'C' THEN 1 ELSE 0 END) AS GearCount,
                SUM(CASE WHEN ShuttleGear = 'C' THEN 1 ELSE 0 END) AS ShuttleGearCount,
			    SUM(CASE WHEN OthersDD = 'C' THEN 1 ELSE 0 END) AS OthersDDCount,
			    SUM(CASE WHEN DCVAvailablity = 'C' THEN 1 ELSE 0 END) AS DCVAvailablityCount,
			    SUM(CASE WHEN DigitalInstrumentCluster = 'C' THEN 1 ELSE 0 END) AS DigitalInstrumentClusterCount,
			    SUM(CASE WHEN MultispeedPTO = 'C' THEN 1 ELSE 0 END) AS MultispeedPTOCount,
			    SUM(CASE WHEN [4WD] = 'C' THEN 1 ELSE 0 END) AS [4WDCount],
			    SUM(CASE WHEN HighLiftCapacity = 'C' THEN 1 ELSE 0 END) AS HighLiftCapacityCount,
        cfg.FYear,
        --pva.status,
		cfg.State,
		cfg.DateFrom
            FROM
        tblCFGCompetitorBenchmarking CB
		inner join tblModel M on cb.SwarajModelId=M.ModelId
		inner join tblModel cM on cb.CompetitorModelId=cM.ModelId
		inner join tblCFGData cfg on cb.CFGId=cfg.CFGId
		inner join tblFeature tP on CB.Id=tP.CBId
        --left outer join tblproductcfgvisitaction pva on cb.id = pva.id
            WHERE
                cfg.DelFlag = 0
                AND IsDel = 0
                AND (PowerSteering = 'C' OR IPTOClutch = 'C' OR Gear = 'C' OR ShuttleGear = 'C' OR OthersDD = 'C' or DCVAvailablity= 'C' OR DigitalInstrumentCluster= 'C' OR MultispeedPTO= 'C' OR HighLiftCapacity= 'C' OR [4WD]= 'C' )
	            --and (@Fyear='ALL'  or cfg.FYear in (SELECT VALUE FROM String_split(@Fyear,',')))
   			    --and (@Status='ALL'  or pva.Status in (SELECT VALUE FROM String_split(@Status,',')))
    GROUP BY
        m.modelname,FYear, cfg.State, cfg.DateFrom;
--select * from @query1

DECLARE @query2 TABLE (
    ModelName NVARCHAR(255),
    FYear NVARCHAR(50),
	State nvarchar(50),
	DateFrom date,
    PowerSteering NVARCHAR(255),
    IPTOClutch NVARCHAR(255),
    Gear NVARCHAR(255),
    ShuttleGear NVARCHAR(255),
    OthersDD NVARCHAR(255),
    DCVAvailablity NVARCHAR(255),
    DigitalInstrumentCluster NVARCHAR(255),
    MultispeedPTO NVARCHAR(255),
    [4WD] NVARCHAR(255),
    HighLiftCapacity NVARCHAR(255),
    PowerSteeringCount INT,
    IPTOClutchCount INT,
    GearCount INT,
    ShuttleGearCount INT,
    OthersDDCount INT,
    DCVAvailablityCount INT,
    DigitalInstrumentClusterCount INT,
    MultispeedPTOCount INT,
    [4WDCount] INT,
    HighLiftCapacityCount INT
);

INSERT INTO @query2
SELECT
    ModelName,
    FYear,
	state,
	DateFrom,
    CASE WHEN Min(PowerSteering) = 'C' THEN 'Power Steering' ELSE '' END AS PowerSteering,
    CASE WHEN Min(IPTOClutch) = 'C' THEN 'IPTO Clutch' ELSE '' END AS IPTOClutch,
    CASE WHEN Min(Gear) = 'C' THEN 'Multispeed Gears' ELSE '' END AS Gear,
    CASE WHEN Min(ShuttleGear) = 'C' THEN 'Shuttle Gear' ELSE '' END AS ShuttleGear,
    CASE WHEN Min(OthersDD) = 'C' THEN 'Shuttle Gears' ELSE '' END AS OthersDD,
    CASE WHEN Min(DCVAvailablity) = 'C' THEN 'DCV Availibility' ELSE '' END AS DCVAvailablity,
    CASE WHEN Min(DigitalInstrumentCluster) = 'C' THEN 'Digital Instrument cluster' ELSE '' END AS DigitalInstrumentCluster,
    CASE WHEN Min(MultispeedPTO) = 'C' THEN 'Multi speed PTo' ELSE '' END AS MultispeedPTO,
    CASE WHEN Min([4WD]) = 'C' THEN '4WD' ELSE '' END AS [4WD],
    CASE WHEN Min(HighLiftCapacity) = 'C' THEN 'High Lift Capacity' ELSE '' END AS HighLiftCapacity,
    SUM(PowerSteeringCount) AS PowerSteeringCount,
    SUM(IPTOClutchCount) AS IPTOClutchCount,
    SUM(GearCount) AS GearCount,
    SUM(ShuttleGearCount) AS ShuttleGearCount,
    SUM(OthersDDCount) AS OthersDDCount,
    SUM(DCVAvailablityCount) AS DCVAvailablityCount,
    SUM(DigitalInstrumentClusterCount) AS DigitalInstrumentClusterCount,
    SUM(MultispeedPTOCount) AS MultispeedPTOCount,
    SUM([4WDCount]) AS [4WDCount],
    SUM(HighLiftCapacityCount) AS HighLiftCapacityCount
FROM
    @query1
GROUP BY
         modelname, fyear,State,DateFrom;	 

select modelname,SubAttribute,Attribute,State,DateFrom,fyear into #d from @query2 as x
		 unpivot
		 (
		 SubAttribute
		 for  Attribute IN(PowerSteering,IPTOClutch,Gear,ShuttleGear,OthersDD,DCVAvailablity,DigitalInstrumentCluster,
		 MultispeedPTO,[4WD],HighLiftCapacity)
		 ) X
		 where SubAttribute!=''
--select * from #d where SubAtttibute!=''--where modelname='724 4WD'

select Attribute,SubAttribute,SwarajModel,Analysis, tb.Id,Status,ClosureDate into #c from tblproductcfgvisitaction 
tb join #tempBenchmarking ts on tb.Id=ts.Id where (Rn=1 or Rn is null) and Status!='No Action Proposed'

select distinct  sm.ModelName as Model, '' as Implement, sm.State as State,'Benchmarking Inferior' AS EffectedAttribute , 
CONCAT('Feature', '-', sm.SubAttribute) AS CasualAttribute,cb.Analysis as Description,Status,fyear,--, sm.SubAttribute--,Status,fyear--, sm.SubAtttibute
CASE
        WHEN MONTH(DateFrom) BETWEEN 1 AND 3 THEN 'Q4'
        WHEN MONTH(DateFrom) BETWEEN 4 AND 6 THEN 'Q1'
        WHEN MONTH(DateFrom) BETWEEN 7 AND 9 THEN 'Q2'
        WHEN MONTH(DateFrom) BETWEEN 10 AND 12 THEN 'Q3'
    END AS [Quarter],
cb.ClosureDate
into #tempp1
from  #d sm LEFT  Join  #c cb  
ON  sm.SubAttribute = cb.SubAttribute and sm.modelname = cb.SwarajModel
--where Status is not null
group by modelname,State,Analysis,CONCAT('Feature', '-', sm.SubAttribute),Status,fyear,--'Benchmarking Inferior',
CASE
        WHEN MONTH(DateFrom) BETWEEN 1 AND 3 THEN 'Q4'
        WHEN MONTH(DateFrom) BETWEEN 4 AND 6 THEN 'Q1'
        WHEN MONTH(DateFrom) BETWEEN 7 AND 9 THEN 'Q2'
        WHEN MONTH(DateFrom) BETWEEN 10 AND 12 THEN 'Q3'
    END,
cb.ClosureDate
order by sm.modelname asc

select * from #tempp1



END;
GO

