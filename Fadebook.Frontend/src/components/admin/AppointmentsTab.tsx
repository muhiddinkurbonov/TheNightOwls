'use client';

import { useState, useEffect } from 'react';
import { useAppointmentsByDate, useUpdateAppointment, useDeleteAppointment } from '@/hooks/useAppointments';
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
import { Calendar, Pencil, Trash2 } from 'lucide-react';
import type { AppointmentDto } from '@/types/api';

export function AppointmentsTab() {
  // Initialize with empty string to avoid hydration mismatch
  const [selectedDate, setSelectedDate] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [editingAppointment, setEditingAppointment] = useState<AppointmentDto | null>(null);
  const [deletingAppointment, setDeletingAppointment] = useState<AppointmentDto | null>(null);
  const [editStatus, setEditStatus] = useState('');

  // Set today's date on client side only to avoid hydration mismatch
  useEffect(() => {
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    setSelectedDate(`${year}-${month}-${day}`);
  }, []);

  const { data: appointments = [], isLoading, error } = useAppointmentsByDate(selectedDate);
  const updateAppointment = useUpdateAppointment();
  const deleteAppointment = useDeleteAppointment();

  const filteredAppointments = statusFilter === 'all'
    ? appointments
    : appointments.filter((apt) => apt.status.toLowerCase() === statusFilter.toLowerCase());

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

  const handleOpenDeleteDialog = (appointment: AppointmentDto) => {
    setDeletingAppointment(appointment);
  };

  const handleCloseDeleteDialog = () => {
    setDeletingAppointment(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingAppointment) return;

    try {
      await deleteAppointment.mutateAsync(deletingAppointment.appointmentId);
      handleCloseDeleteDialog();
    } catch (err) {
      console.error('Failed to delete appointment:', err);
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

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Appointments</CardTitle>
            <CardDescription>View and filter appointments</CardDescription>
          </div>
        </div>
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
            No appointments found for this date
            {statusFilter !== 'all' && ` with status "${statusFilter}"`}
          </p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Date & Time</TableHead>
                <TableHead>Customer</TableHead>
                <TableHead>Barber</TableHead>
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
                    <div className="font-medium">{appointment.barberName || 'Unknown'}</div>
                    <div className="text-xs text-muted-foreground">ID: {appointment.barberId}</div>
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
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenEditDialog(appointment)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenDeleteDialog(appointment)}
                        disabled={deleteAppointment.isPending}
                      >
                        <Trash2 className="h-4 w-4 text-destructive" />
                      </Button>
                    </div>
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

        <Dialog open={!!deletingAppointment} onOpenChange={handleCloseDeleteDialog}>
          <DialogContent className="max-w-md">
            <DialogHeader>
              <DialogTitle>Delete Appointment</DialogTitle>
              <DialogDescription>
                Are you sure you want to delete appointment <strong>#{deletingAppointment?.appointmentId}</strong>? This action cannot be undone.
              </DialogDescription>
            </DialogHeader>
            <div className="flex justify-end gap-3 mt-4">
              <Button variant="outline" onClick={handleCloseDeleteDialog}>
                Cancel
              </Button>
              <Button
                variant="destructive"
                onClick={handleConfirmDelete}
                disabled={deleteAppointment.isPending}
              >
                {deleteAppointment.isPending ? 'Deleting...' : 'Delete Appointment'}
              </Button>
            </div>
          </DialogContent>
        </Dialog>
      </CardContent>
    </Card>
  );
}
