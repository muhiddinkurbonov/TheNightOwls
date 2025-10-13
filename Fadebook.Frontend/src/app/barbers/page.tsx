'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useBarbers } from '@/hooks/useBarbers';
import { useAuth } from '@/providers/AuthProvider';
import { workHoursApi } from '@/lib/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Calendar, Clock } from 'lucide-react';
import type { BarberDto } from '@/types/api';

// Helper component for individual barber card with next available slot
function BarberCard({ barber, canViewPrivateInfo }: { barber: BarberDto; canViewPrivateInfo: boolean }) {
  const router = useRouter();
  const [nextSlot, setNextSlot] = useState<Date | null>(null);
  const [isLoadingSlot, setIsLoadingSlot] = useState(true);

  // Fetch next available slot from the backend (which considers work hours)
  useEffect(() => {
    const findNextAvailableSlot = async () => {
      setIsLoadingSlot(true);
      try {
        const now = new Date();

        // Check next 14 days for an available slot
        for (let day = 0; day < 14; day++) {
          const checkDate = new Date(now);
          checkDate.setDate(checkDate.getDate() + day);
          const dateString = checkDate.toISOString().split('T')[0];

          // Get available slots for this date from the API
          // This API already considers work hours and existing appointments
          const slots = await workHoursApi.getAvailableTimeSlots(barber.barberId, dateString, 30);

          if (slots && slots.length > 0) {
            // Filter out past time slots (for today only)
            const futureSlots = slots.filter(slot => {
              const slotDate = new Date(slot);
              return slotDate > now; // Only include slots in the future
            });

            if (futureSlots.length > 0) {
              // Return the first future available slot
              setNextSlot(new Date(futureSlots[0]));
              setIsLoadingSlot(false);
              return;
            }
          }
        }

        // No slots found in the next 14 days
        setNextSlot(null);
        setIsLoadingSlot(false);
      } catch (error) {
        // Error fetching slots (likely no work hours configured)
        setNextSlot(null);
        setIsLoadingSlot(false);
      }
    };

    findNextAvailableSlot();
  }, [barber.barberId]);

  const handleBookNow = () => {
    // Navigate to booking page with barber pre-selected
    router.push(`/book?barberId=${barber.barberId}`);
  };

  // Don't show barbers without available slots (no work hours configured)
  // Also hide while checking availability to prevent flickering
  if (!nextSlot) {
    return null;
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-start justify-between">
          <div>
            <CardTitle>{barber.name}</CardTitle>
            {canViewPrivateInfo && (
              <CardDescription>@{barber.username}</CardDescription>
            )}
          </div>
          {barber.specialty && (
            <Badge variant="secondary">{barber.specialty}</Badge>
          )}
        </div>
      </CardHeader>
      <CardContent className="space-y-3">
        {canViewPrivateInfo && barber.contactInfo && (
          <p className="text-sm text-muted-foreground">
            Contact: {barber.contactInfo}
          </p>
        )}

        {nextSlot && (
          <div className="flex items-center gap-2 text-sm text-muted-foreground bg-muted/50 p-3 rounded-md">
            <Calendar className="h-4 w-4" />
            <div>
              <p className="font-medium text-foreground">Next available</p>
              <p className="flex items-center gap-1">
                {nextSlot.toLocaleDateString('en-US', {
                  weekday: 'short',
                  month: 'short',
                  day: 'numeric'
                })}
                <Clock className="h-3 w-3 mx-1" />
                {nextSlot.toLocaleTimeString('en-US', {
                  hour: 'numeric',
                  minute: '2-digit',
                  hour12: true
                })}
              </p>
            </div>
          </div>
        )}
      </CardContent>
      <CardFooter>
        <Button onClick={handleBookNow} className="w-full">
          Book Now
        </Button>
      </CardFooter>
    </Card>
  );
}

export default function BarbersPage() {
  const { data: barbers, isLoading, error } = useBarbers();
  const { user, isAuthenticated } = useAuth();
  const [isCheckingAvailability, setIsCheckingAvailability] = useState(true);

  // Only admins and barbers can see contact info and usernames
  const canViewPrivateInfo = user?.role === 'Admin' || user?.role === 'Barber';

  // Don't render barber cards if there are none or still loading
  const shouldRenderCards = !isLoading && !error && barbers && barbers.length > 0;

  // Track when all barber cards have finished checking availability
  useEffect(() => {
    if (shouldRenderCards) {
      // Give barber cards time to check availability (slight delay)
      const timer = setTimeout(() => {
        setIsCheckingAvailability(false);
      }, 2000); // 2 seconds should be enough for most API calls

      return () => clearTimeout(timer);
    } else {
      setIsCheckingAvailability(false);
    }
  }, [shouldRenderCards]);

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-4 sm:py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold mb-4 sm:mb-8">Our Barbers</h1>

          {isLoading && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">Loading barbers...</p>
            </div>
          )}

          {error && (
            <div className="text-center py-12">
              <p className="text-destructive">Error loading barbers. Please try again later.</p>
            </div>
          )}

          {barbers && barbers.length === 0 && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">No barbers found.</p>
            </div>
          )}

          {shouldRenderCards && isCheckingAvailability && (
            <div className="text-center py-12">
              <p className="text-muted-foreground">Checking barber availability...</p>
            </div>
          )}

          {shouldRenderCards && !isCheckingAvailability && (
            <>
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
                {barbers.map((barber) => (
                  <BarberCard key={barber.barberId} barber={barber} canViewPrivateInfo={canViewPrivateInfo} />
                ))}
              </div>
              <p className="text-center text-sm text-muted-foreground mt-6">
                Only showing barbers with available appointments
              </p>
            </>
          )}
        </div>
      </main>
    </div>
  );
}
