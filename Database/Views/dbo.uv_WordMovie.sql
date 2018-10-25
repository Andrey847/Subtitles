IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'uv_WordMovie'))
BEGIN
	EXEC ('CREATE VIEW uv_WordMovie AS SELECT 1 [Default];')
END
GO
-- =======================================================
-- Author:		Andreev
-- Create date: 2018-10-25
-- Single query that returns all words in movie
-- =======================================================
ALTER VIEW dbo.uv_WordMovie
AS
	SELECT DISTINCT 
		m.CustomerId,
		m.MovieId,
		w.WordId,
		w.Source AS WordValue
	FROM dbo.Movie m
		INNER JOIN dbo.Phrase p
			ON m.MovieId = p.MovieId
		INNER JOIN dbo.PhraseWord pw
			ON p.PhraseId = pw.PhraseId
		INNER JOIN dbo.Word w
			ON pw.WordId = w.WordId
				AND w.CustomerId = m.CustomerId
GO