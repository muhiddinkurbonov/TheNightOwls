'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useAuth } from '@/providers/AuthProvider';
import { useServices, useBarbersByService } from '@/hooks/useCustomers';
import { useCreateAppointment } from '@/hooks/useAppointments';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import type { CreateAppointmentDto } from '@/types/api';

export default function BookAppointmentPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  const [customerId, setCustomerId] = useState<number | null>(null);
  const [formData, setFormData] = useState({
    serviceId: '',
    barberId: '',
    appointmentDate: '',
    status: 'Pending',
  });

  useEffect(() => {
    // Redirect to signin if not authenticated
    if (!authLoading && !isAuthenticated) {
      router.push('/signin');
      return;
    }

    // Fetch customer ID from the backend using username
    const fetchCustomerId = async () => {
      if (user?.username) {
        try {
          const response = await fetch(`http://localhost:5288/api/customer/by-username/${user.username}`, {
            credentials: 'include',
            headers: {
              'Authorization': `Bearer ${localStorage.getItem('token')}`,
            },
          });

          if (response.ok) {
            const customer = await response.json();
            setCustomerId(customer.customerId);
          } else {
            console.error('Failed to fetch customer ID');
          }
        } catch (error) {
          console.error('Error fetching customer ID:', error);
        }
      }
    };

    fetchCustomerId();
  }, [user, isAuthenticated, authLoading, router]);

  const { data: services, isLoading: servicesLoading } = useServices();
  const { data: barbers, isLoading: barbersLoading } = useBarbersByService(
    Number(formData.serviceId)
  );
  const createAppointment = useCreateAppointment();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!customerId) {
      alert('Customer ID not found. Please try logging in again.');
      return;
    }

    const appointmentData: CreateAppointmentDto = {
      customerId: customerId,
      serviceId: Number(formData.serviceId),
      barberId: Number(formData.barberId),
      appointmentDate: new Date(formData.appointmentDate).toISOString(),
      status: formData.status,
    };

    try {
      console.log('Sending appointment data:', appointmentData);
      const newAppointment = await createAppointment.mutateAsync(appointmentData);

      // Find service and barber names for success page
      const selectedService = services?.find(s => s.serviceId === Number(formData.serviceId));
      const selectedBarber = barbers?.find(b => b.barberId === Number(formData.barberId));
      const appointmentDateTime = new Date(formData.appointmentDate);

      // Redirect to success page with appointment details in URL
      const qs = new URLSearchParams({
        appointmentId: newAppointment.appointmentId.toString(),
        date: appointmentDateTime.toLocaleDateString(),
        time: appointmentDateTime.toLocaleTimeString(),
        barberName: selectedBarber?.name || 'Unknown',
        serviceName: selectedService?.serviceName || 'Unknown',
      }).toString();
      router.push(`/success?${qs}`);
    } catch (error: any) {
      console.error('Failed to book appointment:', error);
      const errorMessage = error?.response?.data?.message || 'Failed to book appointment. Please try again.';
      alert(errorMessage);
    }
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-8 px-4">
        <div className="container mx-auto max-w-2xl">
          <h1 className="text-4xl font-bold mb-8">Book an Appointment</h1>

          <Card>
            <CardHeader>
              <CardTitle>Appointment Details</CardTitle>
              <CardDescription>Fill in the details to book your appointment</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-6">
                <div className="space-y-2">
                  <Label htmlFor="service">Service</Label>
                  <Select
                    value={formData.serviceId}
                    onValueChange={(value) =>
                      setFormData({ ...formData, serviceId: value, barberId: '' })
                    }
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select a service" />
                    </SelectTrigger>
                    <SelectContent>
                      {servicesLoading && (
                        <SelectItem value="loading" disabled>
                          Loading services...
                        </SelectItem>
                      )}
                      {services?.map((service) => (
                        <SelectItem
                          key={service.serviceId}
                          value={service.serviceId.toString()}
                        >
                          {service.serviceName} - ${service.servicePrice}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="barber">Barber</Label>
                  <Select
                    value={formData.barberId}
                    onValueChange={(value) =>
                      setFormData({ ...formData, barberId: value })
                    }
                    disabled={!formData.serviceId}
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select a barber" />
                    </SelectTrigger>
                    <SelectContent>
                      {barbersLoading && (
                        <SelectItem value="loading" disabled>
                          Loading barbers...
                        </SelectItem>
                      )}
                      {(barbers ?? [])
                        .filter((b): b is NonNullable<typeof b> => Boolean(b))
                        .map((barber) => (
                          <SelectItem
                            key={barber.barberId}
                            value={barber.barberId.toString()}
                          >
                            {barber.name} - {barber.specialty}
                          </SelectItem>
                        ))}
                      {barbers?.length === 0 && (
                        <SelectItem value="none" disabled>
                          No barbers available for this service
                        </SelectItem>
                      )}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="appointmentDate">Date & Time</Label>
                  <Input
                    id="appointmentDate"
                    type="datetime-local"
                    required
                    value={formData.appointmentDate}
                    onChange={(e) =>
                      setFormData({ ...formData, appointmentDate: e.target.value })
                    }
                  />
                </div>

                <Button
                  type="submit"
                  className="w-full"
                  disabled={createAppointment.isPending}
                >
                  {createAppointment.isPending ? 'Booking...' : 'Book Appointment'}
                </Button>
              </form>
            </CardContent>
          </Card>
        </div>
      </main>
    </div>
  );
}
