﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TailSpin.SpaceGame.Web;
using TailSpin.SpaceGame.Web.Models;

namespace Tailspon.SpaceGame.Web.Tests
{

    [TestFixture()]
    public class GameRegionSerivce
    {
        private IDocumentDBRepository<Score> _scoreRepository = new LocalDocumentDBRepository<Score>(@"SampleData/scores.json");

        [TestCase("Milky Way")]
        [TestCase("Andromeda")]
        [TestCase("Pinwheel")]
        [TestCase("NGC 1300")]
        [TestCase("Messier 82")]
        public void FetchOnlyRequestedGameRegion(string gameRegion)
        {
            const int PAGE = 0; // take the first page of results
            const int MAX_RESULTS = 10; // sample up to 10 results

            // Form the query predicate.
            // This expression selects all scores for the provided game region.
            Expression<Func<Score, bool>> queryPredicate = score => (score.GameRegion == gameRegion);

            // Fetch the scores.
            Task<IEnumerable<Score>> scoresTask = _scoreRepository.GetItemsAsync(
                queryPredicate, // the predicate defined above
                score => 1, // we don't care about the order
                PAGE,
                MAX_RESULTS
            );
            IEnumerable<Score> scores = scoresTask.Result;

            // Verify that each score's game region matches the provided game region.
            Assert.That(scores, Is.All.Matches<Score>(score => score.GameRegion == gameRegion));
        }

        [TestCase(0, ExpectedResult = 0)]
        [TestCase(1, ExpectedResult = 1)]
        [TestCase(10, ExpectedResult = 11)]
        public int ReturnRequestedCount(int count)
        {
            const int PAGE = 0; // take the first page of results

            // Fetch the scores.
            Task<IEnumerable<Score>> scoresTask = _scoreRepository.GetItemsAsync(
                score => true, // return all scores
                score => 1, // we don't care about the order
                PAGE,
                count // fetch this number of results
            );
            IEnumerable<Score> scores = scoresTask.Result;

            // Verify that we received the specified number of items.
            //return 0;
            return scores.Count();
        }

    }
}