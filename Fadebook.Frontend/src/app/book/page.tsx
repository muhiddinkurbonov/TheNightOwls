'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { Navigation } from '@/components/Navigation';
import { useAuth } from '@/providers/AuthProvider';
import { useServices, useBarbersByService, useAllCustomers } from '@/hooks/useCustomers';
import { useCreateAppointment } from '@/hooks/useAppointments';
import { workHoursApi } from '@/lib/api';
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
import { Badge } from '@/components/ui/badge';
import { toast } from 'sonner';
import type { CreateAppointmentDto } from '@/types/api';

export default function BookAppointmentPage() {
  const router = useRouter();
  const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  const [customerId, setCustomerId] = useState<number | null>(null);
  const [formData, setFormData] = useState({
    customerId: '', // For barbers to select customer
    serviceId: '',
    barberId: '',
    appointmentDate: '',
    selectedDate: '', // Just the date part (YYYY-MM-DD)
    selectedTimeSlot: '', // The time slot from available slots
    status: 'Pending',
  });
  const [availableSlots, setAvailableSlots] = useState<string[]>([]);
  const [loadingSlots, setLoadingSlots] = useState(false);

  const isBarber = user?.role === 'Barber';

  // IMPORTANT: Always call all hooks before any conditional returns
  const { data: services, isLoading: servicesLoading } = useServices();
  const { data: barbers, isLoading: barbersLoading } = useBarbersByService(
    Number(formData.serviceId)
  );
  const { data: customers, isLoading: customersLoading } = useAllCustomers();
  const createAppointment = useCreateAppointment();

  useEffect(() => {
    // Redirect to signin if not authenticated
    if (!authLoading && !isAuthenticated) {
      router.push('/signin');
      return;
    }

    // Fetch customer ID from the backend using username (only for customers, not barbers)
    const fetchCustomerId = async () => {
      if (user?.username && user?.role === 'Customer') {
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
          }
        } catch (error) {
          // Failed to fetch customer ID
        }
      }
    };

    fetchCustomerId();
  }, [user, isAuthenticated, authLoading, router]);

  // Fetch available time slots when barber and date are selected
  useEffect(() => {
    const fetchAvailableSlots = async () => {
      if (formData.barberId && formData.selectedDate) {
        setLoadingSlots(true);
        try {
          const slots = await workHoursApi.getAvailableTimeSlots(
            Number(formData.barberId),
            formData.selectedDate,
            30 // 30-minute appointment duration
          );
          setAvailableSlots(slots);
        } catch (error) {
          setAvailableSlots([]);
        } finally {
          setLoadingSlots(false);
        }
      } else {
        setAvailableSlots([]);
      }
    };

    fetchAvailableSlots();
  }, [formData.barberId, formData.selectedDate]);

  // Don't render anything while checking auth or if not authenticated
  if (authLoading || !isAuthenticated || !user) {
    return (
      <div className="min-h-screen flex flex-col">
        <Navigation />
        <main className="flex-1 py-4 sm:py-8 px-4">
          <div className="container mx-auto">
            <p className="text-center text-muted-foreground">Loading...</p>
          </div>
        </main>
      </div>
    );
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Determine customer ID: either from logged-in customer or from barber's selection
    const finalCustomerId = isBarber ? Number(formData.customerId) : customerId;

    if (!finalCustomerId) {
      toast.error(isBarber ? 'Please select a customer.' : 'Customer ID not found. Please try logging in again.');
      return;
    }

    if (!formData.selectedTimeSlot) {
      toast.error('Please select a time slot.');
      return;
    }

    // Use the selected time slot directly (it's already in ISO format from the API)
    const appointmentData: CreateAppointmentDto = {
      customerId: finalCustomerId,
      serviceId: Number(formData.serviceId),
      barberId: Number(formData.barberId),
      appointmentDate: formData.selectedTimeSlot,
      status: formData.status,
    };

    try {
      const newAppointment = await createAppointment.mutateAsync(appointmentData);

      // Find service and barber names for success page
      const selectedService = services?.find(s => s.serviceId === Number(formData.serviceId));
      const selectedBarber = barbers?.find(b => b.barberId === Number(formData.barberId));
      const appointmentDateTime = new Date(formData.selectedTimeSlot);

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
      const errorMessage = error?.response?.data?.message || 'Failed to book appointment. Please try again.';
      toast.error(errorMessage);
    }
  };

  return (
    <div className="min-h-screen flex flex-col">
      <Navigation />
      <main className="flex-1 py-4 sm:py-8 px-4">
        <div className="container mx-auto max-w-2xl">
          <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold mb-4 sm:mb-8">Book an Appointment</h1>

          <Card>
            <CardHeader>
              <CardTitle>Appointment Details</CardTitle>
              <CardDescription>Fill in the details to book your appointment</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-6">
                {isBarber && (
                  <div className="space-y-2">
                    <Label htmlFor="customer">Customer</Label>
                    <Select
                      value={formData.customerId}
                      onValueChange={(value) =>
                        setFormData({ ...formData, customerId: value })
                      }
                      required
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="Select a customer" />
                      </SelectTrigger>
                      <SelectContent>
                        {customersLoading && (
                          <SelectItem value="loading" disabled>
                            Loading customers...
                          </SelectItem>
                        )}
                        {customers?.map((customer) => (
                          <SelectItem
                            key={customer.customerId}
                            value={customer.customerId.toString()}
                          >
                            {customer.name} ({customer.username})
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                )}

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
                      setFormData({ ...formData, barberId: value, selectedDate: '', selectedTimeSlot: '' })
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
                  <Label htmlFor="appointmentDate">Select Date</Label>
                  <Input
                    id="appointmentDate"
                    type="date"
                    required
                    disabled={!formData.barberId}
                    min={new Date().toISOString().split('T')[0]}
                    value={formData.selectedDate}
                    onChange={(e) =>
                      setFormData({ ...formData, selectedDate: e.target.value, selectedTimeSlot: '' })
                    }
                  />
                  <p className="text-xs text-muted-foreground">
                    Choose a date to see available time slots
                  </p>
                </div>

                {formData.selectedDate && formData.barberId && (
                  <div className="space-y-2">
                    <Label>Available Time Slots</Label>
                    {loadingSlots ? (
                      <p className="text-sm text-muted-foreground">Loading available slots...</p>
                    ) : availableSlots.length === 0 ? (
                      <p className="text-sm text-muted-foreground">
                        No available time slots for this date. Please choose another date.
                      </p>
                    ) : (
                      <div className="grid grid-cols-3 sm:grid-cols-4 gap-2 max-h-64 overflow-y-auto border rounded-md p-3">
                        {availableSlots.map((slot) => {
                          const slotDate = new Date(slot);
                          const timeString = slotDate.toLocaleTimeString('en-US', {
                            hour: 'numeric',
                            minute: '2-digit',
                            hour12: true,
                          });

                          return (
                            <Button
                              key={slot}
                              type="button"
                              variant={formData.selectedTimeSlot === slot ? 'default' : 'outline'}
                              className="w-full text-xs sm:text-sm"
                              onClick={() =>
                                setFormData({ ...formData, selectedTimeSlot: slot })
                              }
                            >
                              {timeString}
                            </Button>
                          );
                        })}
                      </div>
                    )}
                  </div>
                )}

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
