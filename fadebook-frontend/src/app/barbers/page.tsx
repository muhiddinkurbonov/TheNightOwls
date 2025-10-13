'use client';

import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useBarbers } from '@/hooks/useBarbers';
import { useAuth } from '@/providers/AuthProvider';
import { useAppointmentsByBarberId } from '@/hooks/useAppointments';
import { Card, CardContent, CardDescription, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Calendar, Clock } from 'lucide-react';
import type { BarberDto } from '@/types/api';

// Helper component for individual barber card with next available slot
function BarberCard({ barber, canViewPrivateInfo }: { barber: BarberDto; canViewPrivateInfo: boolean }) {
  const router = useRouter();
  // Always call hooks in the same order - React Query will handle cancellation
  const { data: appointments = [], isLoading: appointmentsLoading } = useAppointmentsByBarberId(barber.barberId);

  // Calculate next available slot (simplified - next hour that's not booked)
  const getNextAvailableSlot = () => {
    // Don't calculate if still loading appointments
    if (appointmentsLoading || !appointments) {
      return null;
    }
    const now = new Date();
    const tomorrow = new Date(now);
    tomorrow.setDate(tomorrow.getDate() + 1);
    tomorrow.setHours(9, 0, 0, 0); // Start at 9 AM tomorrow

    // Check next 7 days for an available slot
    for (let day = 0; day < 7; day++) {
      const checkDate = new Date(tomorrow);
      checkDate.setDate(checkDate.getDate() + day);

      // Check each hour from 9 AM to 5 PM
      for (let hour = 9; hour <= 17; hour++) {
        checkDate.setHours(hour, 0, 0, 0);

        // Check if this slot is booked
        const isBooked = appointments.some(apt => {
          const aptDate = new Date(apt.appointmentDate);
          return (
            aptDate.getFullYear() === checkDate.getFullYear() &&
            aptDate.getMonth() === checkDate.getMonth() &&
            aptDate.getDate() === checkDate.getDate() &&
            aptDate.getHours() === checkDate.getHours() &&
            (apt.status.toLowerCase() === 'pending' || apt.status.toLowerCase() === 'confirmed')
          );
        });

        if (!isBooked) {
          return checkDate;
        }
      }
    }

    return null;
  };

  const nextSlot = getNextAvailableSlot();

  const handleBookNow = () => {
    // Navigate to booking page with barber pre-selected
    router.push(`/book?barberId=${barber.barberId}`);
  };

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

  // Only admins and barbers can see contact info and usernames
  const canViewPrivateInfo = user?.role === 'Admin' || user?.role === 'Barber';

  // Don't render barber cards if there are none or still loading
  const shouldRenderCards = !isLoading && !error && barbers && barbers.length > 0;

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

          {shouldRenderCards && (
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
              {barbers.map((barber) => (
                <BarberCard key={barber.barberId} barber={barber} canViewPrivateInfo={canViewPrivateInfo} />
              ))}
            </div>
          )}
        </div>
      </main>
    </div>
  );
}
