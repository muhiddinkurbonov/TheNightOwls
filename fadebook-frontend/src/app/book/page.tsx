'use client';

import { useState } from 'react';
import { Navigation } from '@/components/Navigation';
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
  const [formData, setFormData] = useState({
    customerId: '',
    serviceId: '',
    barberId: '',
    appointmentDate: '',
    status: 'Pending',
  });

  const { data: services, isLoading: servicesLoading } = useServices();
  const { data: barbers, isLoading: barbersLoading } = useBarbersByService(
    Number(formData.serviceId)
  );
  const createAppointment = useCreateAppointment();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    const appointmentData: CreateAppointmentDto = {
      customerId: Number(formData.customerId),
      serviceId: Number(formData.serviceId),
      barberId: Number(formData.barberId),
      appointmentDate: new Date(formData.appointmentDate).toISOString(),
      status: formData.status,
    };

    try {
      await createAppointment.mutateAsync(appointmentData);
      alert('Appointment booked successfully!');
      setFormData({
        customerId: '',
        serviceId: '',
        barberId: '',
        appointmentDate: '',
        status: 'Pending',
      });
    } catch (error) {
      alert('Failed to book appointment. Please try again.');
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
                  <Label htmlFor="customerId">Customer ID</Label>
                  <Input
                    id="customerId"
                    type="number"
                    required
                    value={formData.customerId}
                    onChange={(e) =>
                      setFormData({ ...formData, customerId: e.target.value })
                    }
                    placeholder="Enter your customer ID"
                  />
                </div>

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
