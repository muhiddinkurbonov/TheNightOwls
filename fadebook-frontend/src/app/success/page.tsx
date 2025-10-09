'use client';

import { useEffect, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { Navigation } from '@/components/Navigation';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { CheckCircle2, Calendar } from 'lucide-react';

export default function SuccessPage() {
  const searchParams = useSearchParams();
  const [appointmentDetails, setAppointmentDetails] = useState({
    appointmentId: '',
    date: '',
    time: '',
    barberName: '',
    serviceName: '',
  });
  const [googleCalendarAdded, setGoogleCalendarAdded] = useState(false);

  const handleAddToGoogleCalendar = () => {
    if (!appointmentDetails.appointmentId) {
      alert('No appointment ID available');
      return;
    }
    // Redirect to backend OAuth flow
    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5288';
    window.location.href = `${apiUrl}/api/GoogleCalendar/google-auth?apptId=${appointmentDetails.appointmentId}`;
  };

  useEffect(() => {
    // Get appointment details from URL params or localStorage
    const appointmentId = searchParams.get('appointmentId') || localStorage.getItem('lastAppointmentId') || '';
    const date = searchParams.get('date') || localStorage.getItem('lastAppointmentDate') || '';
    const time = searchParams.get('time') || localStorage.getItem('lastAppointmentTime') || '';
    const barberName = searchParams.get('barberName') || localStorage.getItem('lastBarberName') || '';
    const serviceName = searchParams.get('serviceName') || localStorage.getItem('lastServiceName') || '';
    const gcalSuccess = searchParams.get('gcal') === 'success';

    setAppointmentDetails({
      appointmentId,
      date,
      time,
      barberName,
      serviceName,
    });

    if (gcalSuccess) {
      setGoogleCalendarAdded(true);
    }

    // Clear localStorage after displaying
    localStorage.removeItem('lastAppointmentId');
    localStorage.removeItem('lastAppointmentDate');
    localStorage.removeItem('lastAppointmentTime');
    localStorage.removeItem('lastBarberName');
    localStorage.removeItem('lastServiceName');
  }, [searchParams]);

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 flex items-center justify-center py-12 px-4">
        <Card className="w-full max-w-2xl">
          <CardHeader className="text-center">
            <div className="flex justify-center mb-4">
              <CheckCircle2 className="h-16 w-16 text-green-500" />
            </div>
            <CardTitle className="text-3xl">Appointment Booked Successfully!</CardTitle>
            <CardDescription>
              Your appointment has been confirmed. We look forward to seeing you!
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-6">
            {appointmentDetails.appointmentId && (
              <div className="bg-muted/50 rounded-lg p-6 space-y-4">
                <div className="flex justify-between items-center">
                  <span className="text-sm font-medium text-muted-foreground">Appointment ID</span>
                  <Badge variant="secondary">#{appointmentDetails.appointmentId}</Badge>
                </div>

                {appointmentDetails.date && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Date</span>
                    <span className="font-medium">{appointmentDetails.date}</span>
                  </div>
                )}

                {appointmentDetails.time && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Time</span>
                    <span className="font-medium">{appointmentDetails.time}</span>
                  </div>
                )}

                {appointmentDetails.barberName && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Barber</span>
                    <span className="font-medium">{appointmentDetails.barberName}</span>
                  </div>
                )}

                {appointmentDetails.serviceName && (
                  <div className="flex justify-between items-center">
                    <span className="text-sm font-medium text-muted-foreground">Service</span>
                    <span className="font-medium">{appointmentDetails.serviceName}</span>
                  </div>
                )}
              </div>
            )}

            <div className="bg-blue-50 dark:bg-blue-950 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
              <h3 className="font-semibold mb-2">What's Next?</h3>
              <ul className="space-y-2 text-sm text-muted-foreground">
                <li>• You will receive a confirmation email shortly</li>
                <li>• Please arrive 5 minutes before your appointment time</li>
                <li>• If you need to cancel or reschedule, please contact us at least 24 hours in advance</li>
              </ul>
            </div>

            {appointmentDetails.appointmentId && (
              <div className="flex justify-center">
                {googleCalendarAdded ? (
                  <div className="w-full max-w-md bg-green-50 dark:bg-green-950 border border-green-200 dark:border-green-800 rounded-lg p-4 text-center">
                    <CheckCircle2 className="inline-block mr-2 h-5 w-5 text-green-600" />
                    <span className="text-green-700 dark:text-green-300 font-medium">
                      Added to Google Calendar!
                    </span>
                  </div>
                ) : (
                  <Button 
                    onClick={handleAddToGoogleCalendar}
                    variant="outline"
                    className="w-full max-w-md"
                  >
                    <Calendar className="mr-2 h-4 w-4" />
                    Add to Google Calendar
                  </Button>
                )}
              </div>
            )}

            <div className="flex gap-4">
              <Link href="/my-appointments" className="flex-1">
                <Button variant="outline" className="w-full">
                  View My Appointments
                </Button>
              </Link>
              <Link href="/" className="flex-1">
                <Button className="w-full">
                  Back to Home
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </main>
    </div>
  );
}
