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
  const appointments: AppointmentWithDetails[] = appointmentsData.map((apt) => {
    const barber = barbers.find((b) => b.barberId === apt.barberId);
    const service = services.find((s) => s.serviceId === apt.serviceId);
    return {
      ...apt,
      barberName: barber?.name,
      serviceName: service?.serviceName,
      servicePrice: service?.servicePrice,
    };
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
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto">
          <h1 className="text-4xl font-bold mb-8">My Appointments</h1>

          {appointments.length === 0 ? (
            <Card>
              <CardContent className="py-12">
                <div className="text-center">
                  <Calendar className="h-12 w-12 mx-auto text-muted-foreground mb-4" />
                  <p className="text-muted-foreground">You don't have any appointments yet.</p>
                  <p className="text-sm text-muted-foreground mt-2">
                    Book your first appointment to get started!
                  </p>
                </div>
              </CardContent>
            </Card>
          ) : (
            <div className="grid gap-6">
              {appointments.map((appointment) => (
                <Card key={appointment.appointmentId}>
                  <CardHeader>
                    <div className="flex items-center justify-between">
                      <div>
                        <CardTitle className="text-xl">
                          Appointment #{appointment.appointmentId}
                        </CardTitle>
                      </div>
                      <div className="flex items-center gap-3">
                        <Badge variant={getStatusVariant(appointment.status)}>
                          {appointment.status}
                        </Badge>
                        {canCancelAppointment(appointment) && (
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => setCancellingAppointment(appointment)}
                            className="text-destructive border-destructive hover:bg-destructive/10"
                          >
                            <XCircle className="h-4 w-4 mr-2" />
                            Cancel
                          </Button>
                        )}
                      </div>
                    </div>
                    <CardDescription>
                      {new Date(appointment.appointmentDate).toLocaleDateString('en-US', {
                        weekday: 'long',
                        year: 'numeric',
                        month: 'long',
                        day: 'numeric',
                      })}
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div className="flex items-center gap-3">
                        <Clock className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Time</p>
                          <p className="text-sm text-muted-foreground">
                            {new Date(appointment.appointmentDate).toLocaleTimeString('en-US', {
                              hour: '2-digit',
                              minute: '2-digit',
                            })}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <User className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Barber</p>
                          <p className="text-sm text-muted-foreground">
                            {appointment.barberName || `Barber #${appointment.barberId}`}
                          </p>
                        </div>
                      </div>

                      <div className="flex items-center gap-3">
                        <Scissors className="h-5 w-5 text-muted-foreground" />
                        <div>
                          <p className="text-sm font-medium">Service</p>
                          <p className="text-sm text-muted-foreground">
                            {appointment.serviceName || `Service #${appointment.serviceId}`}
                          </p>
                        </div>
                      </div>

                      {appointment.servicePrice && (
                        <div className="flex items-center gap-3">
                          <div className="h-5 w-5 flex items-center justify-center text-muted-foreground font-bold">
                            $
                          </div>
                          <div>
                            <p className="text-sm font-medium">Price</p>
                            <p className="text-sm text-muted-foreground">
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
