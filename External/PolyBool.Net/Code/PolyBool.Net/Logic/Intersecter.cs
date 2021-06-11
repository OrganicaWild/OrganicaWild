using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Polybool.Net.Objects;

namespace Polybool.Net.Logic
{
    public class Intersecter
    {
        private readonly bool selfIntersection;

        private Intersecter(bool selfIntersection)
        {
            this.selfIntersection = selfIntersection;
        }

        private readonly LinkedList eventRoot = new LinkedList();

        private Segment SegmentNew(Point start, Point end)
        {
            return new Segment
            {
                Start = start,
                End = end,
                MyFill = new Fill()
            };
        }

        private void EventAddSegment(Segment segment, bool primary)
        {
            Node evStart = EventAddSegmentStart(segment, primary);
            EventAddSegmentEnd(evStart, segment, primary);
        }

        private void EventAddSegmentEnd(Node evStart, Segment seg, bool primary)
        {

            Node evEnd = LinkedList.Node(new Node
            {
                IsStart = false,
                Pt = seg.End,
                Seg = seg,
                Primary = primary,
                Other = evStart,
            });
            evStart.Other = evEnd;

            EventAdd(evEnd, evStart.Pt);
        }

        private Node EventAddSegmentStart(Segment seg, bool primary)
        {
            Node evStart = LinkedList.Node(new Node
            {
                IsStart = true,
                Pt = seg.Start,
                Seg = seg,
                Primary = primary,
            });
            EventAdd(evStart, seg.End);
            return evStart;
        }

        private void EventAdd(Node ev, Point otherPt)
        {
            eventRoot.InsertBefore(ev, here =>
            {
                // should ev be inserted before here?
                int comp = EventCompare(
                    ev.IsStart, ev.Pt, otherPt,
                    here.IsStart, here.Pt, here.Other.Pt
                );
                return comp < 0;
            });
        }

        private int EventCompare(bool p1IsStart, Point p11, Point p12, bool p2IsStart, Point p21, Point p22)
        {
            // compare the selected points first
            int comp = PointUtils.PointsCompare(p11, p21);
            if (comp != 0)
            {
                return comp;
            }
            // the selected points are the same

            if (PointUtils.PointsSame(p12, p22)) // if the non-selected points are the same too...
            {
                return 0; // then the segments are equal
            }

            if (p1IsStart != p2IsStart) // if one is a start and the other isn"t...
            {
                return p1IsStart ? 1 : -1; // favor the one that isn"t the start
            }

            // otherwise, we"ll have to calculate which one is below the other manually
            return PointUtils.PointAboveOrOnLine(p12,
                p2IsStart ? p21 : p22, // order matters
                p2IsStart ? p22 : p21
            )
                ? 1
                : -1;
        }

        private int StatusCompare(Node ev1, Node ev2)
        {
            Point a1 = ev1.Seg.Start;
            Point a2 = ev1.Seg.End;
            Point b1 = ev2.Seg.Start;
            Point b2 = ev2.Seg.End;

            if (PointUtils.PointsCollinear(a1, b1, b2))
            {
                if (PointUtils.PointsCollinear(a2, b1, b2))
                {
                    return 1;
                }
                return PointUtils.PointAboveOrOnLine(a2, b1, b2) ? 1 : -1;
            }
            return PointUtils.PointAboveOrOnLine(a1, b1, b2) ? 1 : -1;
        }

        private Transition StatusFindSurrounding(LinkedList statusRoot, Node ev)
        {
            return statusRoot.FindTransition((here) =>
            {
                int comp = StatusCompare(ev, here.Ev);
                return comp > 0;
            });
        }

        private Segment SegmentCopy(Point start, Point end, Segment seg)
        {
            return new Segment()
            {
                Start = start,
                End = end,
                MyFill = new Fill()
                {
                    Above = seg.MyFill.Above,
                    Below = seg.MyFill.Below

                }
            };
        }

        private void EventUpdateEnd(Node ev, Point end)
        {
            // slides an end backwards
            //   (start)------------(end)    to:
            //   (start)---(end)


            ev.Other.Remove();
            ev.Seg.End = end;
            ev.Other.Pt = end;
            EventAdd(ev.Other, ev.Pt);
        }

        private void EventDivide(Node ev, Point pt)
        {
            Segment ns = SegmentCopy(pt, ev.Seg.End, ev.Seg);
            EventUpdateEnd(ev, pt);
            EventAddSegment(ns, ev.Primary);
        }

        private Node CheckIntersection(Node ev1, Node ev2)
        {
            // returns the segment equal to ev1, or false if nothing equal

            Segment seg1 = ev1.Seg;
            Segment seg2 = ev2.Seg;
            Point a1 = seg1.Start;
            Point a2 = seg1.End;
            Point b1 = seg2.Start;
            Point b2 = seg2.End;


            IntersectionPoint i = PointUtils.LinesIntersect(a1, a2, b1, b2);

            if (i == null)
            {
                // segments are parallel or coincident

                // if points aren"t collinear, then the segments are parallel, so no intersections
                if (!PointUtils.PointsCollinear(a1, a2, b1))
                {
                    return null;
                }
                // otherwise, segments are on top of each other somehow (aka coincident)

                if (PointUtils.PointsSame(a1, b2) || PointUtils.PointsSame(a2, b1))
                {
                    return null; // segments touch at endpoints... no intersection
                }

                bool a1EquB1 = PointUtils.PointsSame(a1, b1);
                bool a2EquB2 = PointUtils.PointsSame(a2, b2);

                if (a1EquB1 && a2EquB2)
                {
                    return ev2; // segments are exactly equal
                }

                bool a1Between = !a1EquB1 && PointUtils.PointBetween(a1, b1, b2);
                bool a2Between = !a2EquB2 && PointUtils.PointBetween(a2, b1, b2);

                if (a1EquB1)
                {
                    if (a2Between)
                    {
                        //  (a1)---(a2)
                        //  (b1)----------(b2)
                        EventDivide(ev2, a2);
                    }
                    else
                    {
                        //  (a1)----------(a2)
                        //  (b1)---(b2)
                        EventDivide(ev1, b2);
                    }
                    return ev2;
                }
                else if (a1Between)
                {
                    if (!a2EquB2)
                    {
                        // make a2 equal to b2
                        if (a2Between)
                        {
                            //         (a1)---(a2)
                            //  (b1)-----------------(b2)
                            EventDivide(ev2, a2);
                        }
                        else
                        {
                            //         (a1)----------(a2)
                            //  (b1)----------(b2)
                            EventDivide(ev1, b2);
                        }
                    }

                    //         (a1)---(a2)
                    //  (b1)----------(b2)
                    EventDivide(ev2, a1);
                }
            }
            else
            {
                // otherwise, lines intersect at i.pt, which may or may not be between the endpoints

                // is A divided between its endpoints? (exclusive)
                if (i.AlongA == 0)
                {
                    if (i.AlongB == -1) // yes, at exactly b1
                    {
                        EventDivide(ev1, b1);
                    }
                    else if (i.AlongB == 0) // yes, somewhere between B"s endpoints
                    {
                        EventDivide(ev1, i.Pt);
                    }
                    else if (i.AlongB == 1) // yes, at exactly b2
                    {
                        EventDivide(ev1, b2);
                    }
                }

                // is B divided between its endpoints? (exclusive)
                if (i.AlongB == 0)
                {
                    if (i.AlongA == -1) // yes, at exactly a1
                    {
                        EventDivide(ev2, a1);
                    }
                    else if (i.AlongA == 0) // yes, somewhere between A"s endpoints (exclusive)
                    {
                        EventDivide(ev2, i.Pt);
                    }
                    else if (i.AlongA == 1) // yes, at exactly a2
                    {
                        EventDivide(ev2, a2);
                    }
                }
            }
            return null;
        }

        private Node CheckBothIntersections(Node above, Node ev, Node below)
        {
            if (above != null)
            {
                Node eve = CheckIntersection(ev, above);
                if (eve != null)
                {
                    return eve;
                }
            }
            if (below != null)
            {
                return CheckIntersection(ev, below);
            }
            return null;
        }

        [SuppressMessage("ReSharper", "PossibleInvalidOperationException")]
        private List<Segment> Calculate(bool primaryPolyInverted, bool secondaryPolyInverted)
        {
            // if selfIntersection is true then there is no secondary polygon, so that isn"t used

            //
            // status logic
            //

            LinkedList statusRoot = new LinkedList();



            //
            // main event loop
            //
            List<Segment> segments = new List<Segment>();
            while (!eventRoot.IsEmpty())
            {
                Node ev = eventRoot.GetHead();


                if (ev.IsStart)
                {


                    Transition surrounding = StatusFindSurrounding(statusRoot, ev);
                    Node above = surrounding.Before != null ? surrounding.Before.Ev : null;
                    Node below = surrounding.After != null ? surrounding.After.Ev : null;


                    Node eve = CheckBothIntersections(above, ev, below);
                    if (eve != null)
                    {
                        // ev and eve are equal
                        // we"ll keep eve and throw away ev

                        // merge ev.seg"s fill information into eve.seg

                        if (selfIntersection)
                        {
                            bool toggle; // are we a toggling edge?
                            if (ev.Seg.MyFill.Below == null)
                            {
                                toggle = true;
                            }
                            else
                            {
                                toggle = ev.Seg.MyFill.Above != ev.Seg.MyFill.Below;
                            }

                            // merge two segments that belong to the same polygon
                            // think of this as sandwiching two segments together, where `eve.seg` is
                            // the bottom -- this will cause the above fill flag to toggle
                            if (toggle)
                            {
                                eve.Seg.MyFill.Above = !eve.Seg.MyFill.Above;
                            }
                        }
                        else
                        {
                            // merge two segments that belong to different polygons
                            // each segment has distinct knowledge, so no special logic is needed
                            // note that this can only happen once per segment in this phase, because we
                            // are guaranteed that all self-intersections are gone
                            eve.Seg.OtherFill = ev.Seg.MyFill;
                        }

                        ev.Other.Remove();
                        ev.Remove();
                    }

                    if (!Equals(eventRoot.GetHead(), ev))
                    {
                        // something was inserted before us in the event queue, so loop back around and
                        // process it before continuing
                        continue;
                    }

                    //
                    // calculate fill flags
                    //
                    if (selfIntersection)
                    {
                        bool toggle; // are we a toggling edge?
                        if (ev.Seg.MyFill.Below == null) // if we are a new segment...
                        {
                            toggle = true; // then we toggle
                        }
                        else // we are a segment that has previous knowledge from a division
                        {
                            toggle = ev.Seg.MyFill.Above != ev.Seg.MyFill.Below; // calculate toggle
                        }

                        // next, calculate whether we are filled below us
                        if (below == null)
                        {
                            // if nothing is below us...
                            // we are filled below us if the polygon is inverted
                            ev.Seg.MyFill.Below = primaryPolyInverted;
                        }
                        else
                        {
                            // otherwise, we know the answer -- it"s the same if whatever is below
                            // us is filled above it
                            ev.Seg.MyFill.Below = below.Seg.MyFill.Above;
                        }

                        // since now we know if we"re filled below us, we can calculate whether
                        // we"re filled above us by applying toggle to whatever is below us
                        if (toggle)
                        {
                            ev.Seg.MyFill.Above = !ev.Seg.MyFill.Below;
                        }
                        else
                        {
                            ev.Seg.MyFill.Above = ev.Seg.MyFill.Below;
                        }
                    }
                    else
                    {
                        // now we fill in any missing transition information, since we are all-knowing
                        // at this point

                        if (ev.Seg.OtherFill == null)
                        {
                            // if we don"t have other information, then we need to figure out if we"re
                            // inside the other polygon
                            bool inside;
                            if (below == null)
                            {
                                // if nothing is below us, then we"re inside if the other polygon is
                                // inverted
                                inside =
                                    ev.Primary ? secondaryPolyInverted : primaryPolyInverted;
                            }
                            else
                            {
                                // otherwise, something is below us
                                // so copy the below segment"s other polygon"s above
                                if (ev.Primary == below.Primary)
                                {
                                    inside = below.Seg.OtherFill.Above.Value;
                                }
                                else
                                {
                                    inside = below.Seg.MyFill.Above.Value;
                                }
                            }
                            ev.Seg.OtherFill = new Fill()
                            {
                                Above = inside,
                                Below = inside
                            };
                        }
                    }


                    // insert the status and remember it for later removal
                    ev.Other.Status = surrounding.Insert(LinkedList.Node(new Node() { Ev = ev }));
                }
                else
                {
                    Node st = ev.Status;

                    if (st == null)
                    {
                        throw new Exception("PolyBool: Zero-length segment detected; your epsilon is " +
                                            "probably too small or too large");
                    }

                    // removing the status will create two new adjacent edges, so we"ll need to check
                    // for those
                    if (statusRoot.Exists(st.Previous) && statusRoot.Exists(st.Next))
                    {
                        CheckIntersection(st.Previous.Ev, st.Next.Ev);
                    }


                    // remove the status
                    st.Remove();

                    // if we"ve reached this point, we"ve calculated everything there is to know, so
                    // save the segment for reporting
                    if (!ev.Primary)
                    {
                        // make sure `seg.myFill` actually points to the primary polygon though
                        Fill s = ev.Seg.MyFill;
                        ev.Seg.MyFill = ev.Seg.OtherFill;
                        ev.Seg.OtherFill = s;
                    }
                    segments.Add(ev.Seg);
                }

                // remove the event and continue
                eventRoot.GetHead().Remove();
            }



            return segments;
        }

        public class RegionIntersecter : Intersecter
        {
            public void AddRegion(Region region)
            {
                // regions are a list of points:
                //  [ [0, 0], [100, 0], [50, 100] ]
                // you can add multiple regions before running calculate
                Point pt1;
                Point pt2 = region.Points[region.Points.Count - 1];
                for (int i = 0; i < region.Points.Count; i++)
                {
                    pt1 = pt2;
                    pt2 = region.Points[i];

                    int forward = PointUtils.PointsCompare(pt1, pt2);
                    if (forward == 0) // points are equal, so we have a zero-length segment
                    {
                        continue; // just skip it
                    }

                    EventAddSegment(
                        SegmentNew(
                            forward < 0 ? pt1 : pt2,
                            forward < 0 ? pt2 : pt1
                        ),
                        true
                    );
                }
            }

            internal List<Segment> Calculate(bool inverted)
            {
                return Calculate(inverted, false);
            }

            public RegionIntersecter() : base(true)
            {
            }
        }
        public class SegmentIntersecter : Intersecter
        {
            public void AddRegion(Region region)
            {
                // regions are a list of points:
                //  [ [0, 0], [100, 0], [50, 100] ]
                // you can add multiple regions before running calculate
                Point pt1;
                Point pt2 = region.Points[region.Points.Count - 1];
                for (int i = 0; i < region.Points.Count; i++)
                {
                    pt1 = pt2;
                    pt2 = region.Points[i];

                    int forward = PointUtils.PointsCompare(pt1, pt2);
                    if (forward == 0) // points are equal, so we have a zero-length segment
                    {
                        continue; // just skip it
                    }

                    EventAddSegment(
                        SegmentNew(
                            forward < 0 ? pt1 : pt2,
                            forward < 0 ? pt2 : pt1
                        ),
                        true
                    );
                }
            }

            internal List<Segment> Calculate(List<Segment> segments1, bool isInverted1, List<Segment> segments2, bool isInverted2)
            {
                // returns segments that can be used for further operations
                foreach (Segment segment in segments1)
                {
                    EventAddSegment(segment, true);
                }
                foreach (Segment segment in segments2)
                {
                    EventAddSegment(segment, false);
                }

                return Calculate(isInverted1, isInverted2);
            }

            public SegmentIntersecter() : base(false)
            {
            }
        }
    }
}