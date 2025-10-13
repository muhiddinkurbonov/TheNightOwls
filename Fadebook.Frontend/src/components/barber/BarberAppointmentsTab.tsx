'use client';

import { useState } from 'react';
import { useAuth } from '@/providers/AuthProvider';
import { useAppointmentsByBarberId, useUpdateAppointment } from '@/hooks/useAppointments';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Calendar, Pencil } from 'lucide-react';
import type { AppointmentDto } from '@/types/api';

export function BarberAppointmentsTab() {
  const { user } = useAuth();
  const [selectedDate, setSelectedDate] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [editingAppointment, setEditingAppointment] = useState<AppointmentDto | null>(null);
  const [editStatus, setEditStatus] = useState('');

  // Fetch appointments for this barber using their barberId
  const { data: appointments = [], isLoading, error } = useAppointmentsByBarberId(user?.barberId || 0);
  const updateAppointment = useUpdateAppointment();

  const filteredAppointments = appointments.filter((apt) => {
    const matchesDate = !selectedDate || new Date(apt.appointmentDate).toDateString() === new Date(selectedDate).toDateString();
    const matchesStatus = statusFilter === 'all' || apt.status.toLowerCase() === statusFilter.toLowerCase();
    return matchesDate && matchesStatus;
  });

  const handleOpenEditDialog = (appointment: AppointmentDto) => {
    setEditingAppointment(appointment);
    setEditStatus(appointment.status);
  };

  const handleCloseEditDialog = () => {
    setEditingAppointment(null);
    setEditStatus('');
  };

  const handleUpdateStatus = async () => {
    if (!editingAppointment) return;

    try {
      await updateAppointment.mutateAsync({
        id: editingAppointment.appointmentId,
        appointment: {
          customerId: editingAppointment.customerId,
          barberId: editingAppointment.barberId,
          serviceId: editingAppointment.serviceId,
          appointmentDate: editingAppointment.appointmentDate,
          status: editStatus,
        },
      });
      handleCloseEditDialog();
    } catch (err) {
      console.error('Failed to update appointment:', err);
    }
  };

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

  if (!user?.barberId) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>My Appointments</CardTitle>
          <CardDescription>View and manage your customer appointments</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="p-8 text-center space-y-4">
            <p className="text-muted-foreground">
              Unable to load appointments. Your barber profile is not set up correctly.
            </p>
            <p className="text-sm text-muted-foreground">
              Please contact an administrator to ensure a barber record exists for your account.
            </p>
            <p className="text-xs text-muted-foreground">
              Debug info: Username: {user?.username}, Barber ID: {user?.barberId || 'Not found'}
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>My Appointments</CardTitle>
        <CardDescription>View and manage your customer appointments</CardDescription>
      </CardHeader>
      <CardContent className="space-y-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <Label htmlFor="date">Filter by Date</Label>
            <div className="relative">
              <Calendar className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
              <Input
                id="date"
                type="date"
                value={selectedDate}
                onChange={(e) => setSelectedDate(e.target.value)}
                className="pl-8"
              />
            </div>
          </div>
          <div className="w-48">
            <Label htmlFor="status">Filter by Status</Label>
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger id="status">
                <SelectValue placeholder="All statuses" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Statuses</SelectItem>
                <SelectItem value="pending">Pending</SelectItem>
                <SelectItem value="confirmed">Confirmed</SelectItem>
                <SelectItem value="completed">Completed</SelectItem>
                <SelectItem value="cancelled">Cancelled</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>

        {isLoading ? (
          <p className="text-center py-8 text-muted-foreground">Loading appointments...</p>
        ) : error ? (
          <p className="text-center py-8 text-destructive">Failed to load appointments</p>
        ) : filteredAppointments.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">
            No appointments found
            {selectedDate && ' for this date'}
            {statusFilter !== 'all' && ` with status "${statusFilter}"`}
          </p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Date & Time</TableHead>
                <TableHead>Customer</TableHead>
                <TableHead>Service</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredAppointments.map((appointment) => (
                <TableRow key={appointment.appointmentId}>
                  <TableCell>{appointment.appointmentId}</TableCell>
                  <TableCell>
                    {new Date(appointment.appointmentDate).toLocaleString()}
                  </TableCell>
                  <TableCell>
                    <div className="font-medium">{appointment.customerName || 'Unknown'}</div>
                    <div className="text-xs text-muted-foreground">ID: {appointment.customerId}</div>
                  </TableCell>
                  <TableCell>
                    <div className="font-medium">{appointment.serviceName || 'Unknown'}</div>
                    <div className="text-xs text-muted-foreground">ID: {appointment.serviceId}</div>
                  </TableCell>
                  <TableCell>
                    <Badge variant={getStatusVariant(appointment.status)}>
                      {appointment.status}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleOpenEditDialog(appointment)}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        )}

        <Dialog open={!!editingAppointment} onOpenChange={handleCloseEditDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle>Update Appointment Status</DialogTitle>
              <DialogDescription>
                Change the status of appointment #{editingAppointment?.appointmentId}
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="edit-status">Status</Label>
                <Select value={editStatus} onValueChange={setEditStatus}>
                  <SelectTrigger id="edit-status">
                    <SelectValue placeholder="Select status" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Pending">Pending</SelectItem>
                    <SelectItem value="Confirmed">Confirmed</SelectItem>
                    <SelectItem value="Completed">Completed</SelectItem>
                    <SelectItem value="Cancelled">Cancelled</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <Button
                onClick={handleUpdateStatus}
                className="w-full"
                disabled={updateAppointment.isPending}
              >
                {updateAppointment.isPending ? 'Updating...' : 'Update Status'}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </CardContent>
    </Card>
  );
}
