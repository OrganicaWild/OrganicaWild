using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Polybool.Net.Objects;

namespace Polybool.Net.Logic
{
    public static class PolyBool
    {
        internal static List<Region> SegmentChainer(List<Segment> segments)
        {
            List<Region> regions = new List<Region>();
            List<List<Point>> chains = new List<List<Point>>();

            foreach (Segment seg in segments)
            {
                Point pt1 = seg.Start;
                Point pt2 = seg.End;
                if (PointUtils.PointsSame(pt1, pt2))
                {
                    Debug.WriteLine("PolyBool: Warning: Zero-length segment detected; your epsilon is " +
                        "probably too small or too large");
                    continue;
                }

                // search for two chains that this segment matches
                Matcher firstMatch = new Matcher()
                {
                    Index = 0,
                    MatchesHead = false,
                    MatchesPt1 = false

                };
                Matcher secondMatch = new Matcher()
                {
                    Index = 0,
                    MatchesHead = false,
                    MatchesPt1 = false

                };
                Matcher nextMatch = firstMatch;

                Func<int, bool, bool, bool> setMatch = (index, matchesHead, matchesPt1) =>
                 {
                     // return true if we've matched twice
                     nextMatch.Index = index;
                     nextMatch.MatchesHead = matchesHead;
                     nextMatch.MatchesPt1 = matchesPt1;
                     if (Equals(nextMatch, firstMatch))
                     {
                         nextMatch = secondMatch;
                         return false;
                     }
                     nextMatch = null;
                     return true; // we've matched twice, we're done here
                 };


                for (int i = 0; i < chains.Count; i++)
                {
                    List<Point> chain = chains[i];
                    Point head = chain[0];
                    Point tail = chain[chain.Count - 1];
                    if (PointUtils.PointsSame(head, pt1))
                    {
                        if (setMatch(i, true, true))
                        {
                            break;
                        }
                    }
                    else if (PointUtils.PointsSame(head, pt2))
                    {
                        if (setMatch(i, true, false))
                        {
                            break;
                        }
                    }
                    else if (PointUtils.PointsSame(tail, pt1))
                    {
                        if (setMatch(i, false, true))
                        {
                            break;
                        }
                    }
                    else if (PointUtils.PointsSame(tail, pt2))
                    {
                        if (setMatch(i, false, false))
                        {
                            break;
                        }
                    }
                }

                if (Equals(nextMatch, firstMatch))
                {
                    // we didn't match anything, so create a new chain
                    chains.Add(new List<Point> { pt1, pt2 });
                    continue;
                }

                if (Equals(nextMatch, secondMatch))
                {
                    // we matched a single chain

                    // add the other point to the apporpriate end, and check to see if we've closed the
                    // chain into a loop

                    int index = firstMatch.Index;
                    Point pt = firstMatch.MatchesPt1 ? pt2 : pt1; // if we matched pt1, then we add pt2, etc
                    bool addToHead = firstMatch.MatchesHead; // if we matched at head, then add to the head

                    List<Point> chain = chains[index];
                    Point grow = addToHead ? chain[0] : chain[chain.Count - 1];
                    Point grow2 = addToHead ? chain[1] : chain[chain.Count - 2];
                    Point oppo = addToHead ? chain[chain.Count - 1] : chain[0];
                    Point oppo2 = addToHead ? chain[chain.Count - 2] : chain[1];

                    if (PointUtils.PointsCollinear(grow2, grow, pt))
                    {
                        // grow isn't needed because it's directly between grow2 and pt:
                        // grow2 ---grow---> pt
                        if (addToHead)
                        {
                            chain.Shift();
                        }
                        else
                        {
                            chain.Pop();
                        }
                        grow = grow2; // old grow is gone... new grow is what grow2 was
                    }

                    if (PointUtils.PointsSame(oppo, pt))
                    {
                        // we're closing the loop, so remove chain from chains
                        chains.Splice(index, 1);

                        if (PointUtils.PointsCollinear(oppo2, oppo, grow))
                        {
                            // oppo isn't needed because it's directly between oppo2 and grow:
                            // oppo2 ---oppo--->grow
                            if (addToHead)
                            {
                                chain.Pop();
                            }
                            else
                            {
                                chain.Shift();
                            }
                        }

                        // we have a closed chain!
                        regions.Add(new Region() { Points = chain.ToList() });
                        continue;
                    }

                    // not closing a loop, so just add it to the apporpriate side
                    if (addToHead)
                    {
                        chain.Unshift(pt);
                    }
                    else
                    {
                        chain.Push(pt);
                    }
                    continue;
                }

                // otherwise, we matched two chains, so we need to combine those chains together

                Action<int> reverseChain = (index) =>
                {
                    chains[index].Reverse(); // gee, that's easy
                };

                Action<int, int> appendChain = (index1, index2) =>
                {
                    // index1 gets index2 appended to it, and index2 is removed
                    List<Point> chain1 = chains[index1];
                    List<Point> chain2 = chains[index2];
                    Point tail = chain1[chain1.Count - 1];
                    Point tail2 = chain1[chain1.Count - 2];
                    Point head = chain2[0];
                    Point head2 = chain2[1];

                    if (PointUtils.PointsCollinear(tail2, tail, head))
                    {
                        // tail isn't needed because it's directly between tail2 and head
                        // tail2 ---tail---> head
                        chain1.Pop();
                        tail = tail2; // old tail is gone... new tail is what tail2 was
                    }

                    if (PointUtils.PointsCollinear(tail, head, head2))
                    {
                        // head isn't needed because it's directly between tail and head2
                        // tail ---head---> head2
                        chain2.Shift();
                    }
                    chains[index1] = chain1.Concat(chain2).ToList();
                    chains.Splice(index2, 1);
                };

                int f = firstMatch.Index;
                int s = secondMatch.Index;


                bool reverseF = chains[f].Count < chains[s].Count; // reverse the shorter chain, if needed
                if (firstMatch.MatchesHead)
                {
                    if (secondMatch.MatchesHead)
                    {
                        if (reverseF)
                        {
                            // <<<< F <<<< --- >>>> S >>>>
                            reverseChain(f);
                            // >>>> F >>>> --- >>>> S >>>>
                            appendChain(f, s);
                        }
                        else
                        {
                            // <<<< F <<<< --- >>>> S >>>>
                            reverseChain(s);
                            // <<<< F <<<< --- <<<< S <<<<   logically same as:
                            // >>>> S >>>> --- >>>> F >>>>
                            appendChain(s, f);
                        }
                    }
                    else
                    {
                        // <<<< F <<<< --- <<<< S <<<<   logically same as:
                        // >>>> S >>>> --- >>>> F >>>>
                        appendChain(s, f);
                    }
                }
                else
                {
                    if (secondMatch.MatchesHead)
                    {
                        // >>>> F >>>> --- >>>> S >>>>
                        appendChain(f, s);
                    }
                    else
                    {
                        if (reverseF)
                        {
                            // >>>> F >>>> --- <<<< S <<<<
                            reverseChain(f);
                            // <<<< F <<<< --- <<<< S <<<<   logically same as:
                            // >>>> S >>>> --- >>>> F >>>>
                            appendChain(s, f);
                        }
                        else
                        {
                            // >>>> F >>>> --- <<<< S <<<<
                            reverseChain(s);
                            // >>>> F >>>> --- >>>> S >>>>
                            appendChain(f, s);
                        }
                    }
                }
            }

            return regions;
        }

        public static PolySegments Segments(Polygon poly)
        {
            Intersecter.RegionIntersecter i = new Intersecter.RegionIntersecter();
            foreach (Region region in poly.Regions)
            {
                i.AddRegion(region);
            }

            return new PolySegments
            {
                Segments = i.Calculate(poly.Inverted),
                IsInverted = poly.Inverted
            };
        }

        public static CombinedPolySegments Combine(PolySegments segments1, PolySegments segments2)
        {
            Intersecter.SegmentIntersecter i = new Intersecter.SegmentIntersecter();
            return new CombinedPolySegments
            {
                Combined = i.Calculate(segments1.Segments, segments1.IsInverted, segments2.Segments, segments2.IsInverted),
                IsInverted1 = segments1.IsInverted,
                IsInverted2 = segments2.IsInverted

            };
        }

        public static Polygon Polygon(PolySegments polySegments)
        {
            return new Polygon
            {
                Regions = SegmentChainer(polySegments.Segments),
                Inverted = polySegments.IsInverted
            };
        }


    }
}