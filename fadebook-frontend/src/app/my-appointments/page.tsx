'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useAuth } from '@/providers/AuthProvider';
import { useAppointmentsByUsername, useUpdateAppointment } from '@/hooks/useAppointments';
import { useBarbers } from '@/hooks/useBarbers';
import { useServices } from '@/hooks/useServices';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { Calendar, Clock, User, Scissors, XCircle } from 'lucide-react';
import type { AppointmentDto, BarberDto, ServiceDto } from '@/types/api';

interface AppointmentWithDetails extends AppointmentDto {
  barberName?: string;
  serviceName?: string;
  servicePrice?: number;
}

export default function MyAppointmentsPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  const [cancellingAppointment, setCancellingAppointment] = useState<AppointmentDto | null>(null);

  // Auto-refetching hooks
  const { data: appointmentsData = [], isLoading: appointmentsLoading, error: appointmentsError } = useAppointmentsByUsername(user?.username || '');
  const { data: barbers = [] } = useBarbers();
  const { data: services = [] } = useServices();
  const updateAppointment = useUpdateAppointment();

  // Redirect if not authenticated
  useEffect(() => {
    if (authLoading) return;
    if (!isAuthenticated || !user) {
      router.push('/signin');
    }
  }, [user, isAuthenticated, authLoading, router]);

  // Don't render anything while checking auth or if not authenticated
  if (authLoading || !isAuthenticated || !user) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-muted-foreground">Loading...</p>
          </div>
        </main>
      </div>
    );
  }

  // Combine appointments with barber and service details
  const appointmentsWithDetails: AppointmentWithDetails[] = appointmentsData.map((apt) => {
    const barber = barbers.find((b) => b.barberId === apt.barberId);
    const service = services.find((s) => s.serviceId === apt.serviceId);
    return {
      ...apt,
      barberName: barber?.name,
      serviceName: service?.serviceName,
      servicePrice: service?.servicePrice,
    };
  });

  // Sort appointments logically:
  // 1. Upcoming (Pending/Confirmed) - soonest first
  // 2. Past (Completed) - most recent first
  // 3. Cancelled - most recent first
  const appointments = appointmentsWithDetails.sort((a, b) => {
    const now = new Date();
    const dateA = new Date(a.appointmentDate);
    const dateB = new Date(b.appointmentDate);
    const statusA = a.status.toLowerCase();
    const statusB = b.status.toLowerCase();

    // Define status priority
    const getStatusPriority = (status: string, date: Date) => {
      if (status === 'pending' || status === 'confirmed') {
        return date >= now ? 1 : 2; // Upcoming = 1, Past active = 2
      }
      if (status === 'completed') return 3;
      if (status === 'cancelled') return 4;
      return 5;
    };

    const priorityA = getStatusPriority(statusA, dateA);
    const priorityB = getStatusPriority(statusB, dateB);

    // First sort by priority
    if (priorityA !== priorityB) {
      return priorityA - priorityB;
    }

    // Within same priority, sort by date
    if (priorityA === 1) {
      // Upcoming: soonest first (ascending)
      return dateA.getTime() - dateB.getTime();
    } else {
      // Past/Cancelled: most recent first (descending)
      return dateB.getTime() - dateA.getTime();
    }
  });

  const isLoading = appointmentsLoading;
  const error = appointmentsError ? 'Failed to load appointments' : '';

  const getStatusVariant = (status: string) => {
    switch (status.toLowerCase()) {
      case 'confirmed':
        return 'default';
      case 'pending':
        return 'secondary';
      case 'cancelled':
        return 'destructive';
      case 'completed':
        return 'outline';
      default:
        return 'secondary';
    }
  };

  const handleCancelAppointment = async () => {
    if (!cancellingAppointment) return;

    try {
      await updateAppointment.mutateAsync({
        id: cancellingAppointment.appointmentId,
        appointment: {
          customerId: cancellingAppointment.customerId,
          barberId: cancellingAppointment.barberId,
          serviceId: cancellingAppointment.serviceId,
          appointmentDate: cancellingAppointment.appointmentDate,
          status: 'Cancelled',
        },
      });

      setCancellingAppointment(null);
    } catch (err) {
      console.error('Failed to cancel appointment:', err);
      alert('Failed to cancel appointment. Please try again.');
    }
  };

  const canCancelAppointment = (appointment: AppointmentDto) => {
    const status = appointment.status.toLowerCase();
    return status === 'pending' || status === 'confirmed';
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-muted-foreground">Loading your appointments...</p>
          </div>
        </main>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-destructive">{error}</p>
          </div>
        </main>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-4 sm:py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold mb-4 sm:mb-8">My Appointments</h1>

          {appointments.length === 0 ? (
            <Card>
              <CardContent className="py-8 sm:py-12">
                <div className="text-center">
                  <Calendar className="h-10 w-10 sm:h-12 sm:w-12 mx-auto text-muted-foreground mb-3 sm:mb-4" />
                  <p className="text-sm sm:text-base text-muted-foreground">You don't have any appointments yet.</p>
                  <p className="text-xs sm:text-sm text-muted-foreground mt-2">
                    Book your first appointment to get started!
                  </p>
                </div>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-4 sm:gap-6">
              {appointments.map((appointment) => (
                <Card key={appointment.appointmentId}>
                  <CardHeader>
                    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3">
                      <div>
                        <CardTitle className="text-lg sm:text-xl">
                          Appointment #{appointment.appointmentId}
                        </CardTitle>
                      </div>
                      <div className="flex items-center gap-2 sm:gap-3">
                        <Badge variant={getStatusVariant(appointment.status)}>
                          {appointment.status}
                        </Badge>
                        {canCancelAppointment(appointment) && (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setCancellingAppointment(appointment)}
                            className="text-destructive border-destructive hover:bg-destructive/10 text-xs sm:text-sm"
                          >
                            <XCircle className="h-3 w-3 sm:h-4 sm:w-4 mr-1 sm:mr-2" />
                            Cancel
                          </Button>
                        )}
                      </div>
                    </div>
                    <CardDescription className="text-xs sm:text-sm">
                      {new Date(appointment.appointmentDate).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                      <div className="flex items-center gap-2 sm:gap-3">
                        <Clock className="h-4 w-4 sm:h-5 sm:w-5 text-muted-foreground flex-shrink-0" />
                        <div>
                          <p className="text-xs sm:text-sm font-medium">Time</p>
                          <p className="text-xs sm:text-sm text-muted-foreground">
                            {new Date(appointment.appointmentDate).toLocaleTimeString('en-US', {
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-2 sm:gap-3">
                        <User className="h-4 w-4 sm:h-5 sm:w-5 text-muted-foreground flex-shrink-0" />
                        <div>
                          <p className="text-xs sm:text-sm font-medium">Barber</p>
                          <p className="text-xs sm:text-sm text-muted-foreground">
                            {appointment.barberName || `Barber #${appointment.barberId}`}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-2 sm:gap-3">
                        <Scissors className="h-4 w-4 sm:h-5 sm:w-5 text-muted-foreground flex-shrink-0" />
                        <div>
                          <p className="text-xs sm:text-sm font-medium">Service</p>
                          <p className="text-xs sm:text-sm text-muted-foreground">
                            {appointment.serviceName || `Service #${appointment.serviceId}`}
                          </p>
                        </div>
                      </div>

                      {appointment.servicePrice && (
                        <div className="flex items-center gap-2 sm:gap-3">
                          <div className="h-4 w-4 sm:h-5 sm:w-5 flex items-center justify-center text-muted-foreground font-bold flex-shrink-0">
                            $
                          </div>
                          <div>
                            <p className="text-xs sm:text-sm font-medium">Price</p>
                            <p className="text-xs sm:text-sm text-muted-foreground">
                              ${appointment.servicePrice.toFixed(2)}
                            </p>
                          </div>
                        </div>
                      )}
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>
          )}

          <AlertDialog open={!!cancellingAppointment} onOpenChange={() => setCancellingAppointment(null)}>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>Cancel Appointment</AlertDialogTitle>
                <AlertDialogDescription>
                  Are you sure you want to cancel appointment #{cancellingAppointment?.appointmentId}?
                  This action cannot be undone. You will need to book a new appointment if you change your mind.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>Keep Appointment</AlertDialogCancel>
                <AlertDialogAction
                  onClick={handleCancelAppointment}
                  className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                  disabled={updateAppointment.isPending}
                >
                  {updateAppointment.isPending ? 'Cancelling...' : 'Yes, Cancel Appointment'}
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </div>
      </main>
    </div>
  );
}
