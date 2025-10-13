'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { workHoursApi, barbersApi } from '@/lib/api';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Badge } from '@/components/ui/badge';
import { Checkbox } from '@/components/ui/checkbox';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Plus, Trash2, Pencil } from 'lucide-react';
import type { BarberWorkHoursDto, CreateBarberWorkHoursDto } from '@/types/api';

const daysOfWeek = [
  { value: 0, label: 'Sunday' },
  { value: 1, label: 'Monday' },
  { value: 2, label: 'Tuesday' },
  { value: 3, label: 'Wednesday' },
  { value: 4, label: 'Thursday' },
  { value: 5, label: 'Friday' },
  { value: 6, label: 'Saturday' },
];

export function WorkHoursTab() {
  const queryClient = useQueryClient();
  const { data: workHours, isLoading, error } = useQuery({
    queryKey: ['workHours'],
    queryFn: workHoursApi.getAll,
  });
  const { data: barbers } = useQuery({
    queryKey: ['barbers'],
    queryFn: barbersApi.getAll,
  });

  const createMutation = useMutation({
    mutationFn: workHoursApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['workHours'] });
    },
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, data }: { id: number; data: CreateBarberWorkHoursDto }) =>
      workHoursApi.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['workHours'] });
    },
  });

  const deleteMutation = useMutation({
    mutationFn: workHoursApi.delete,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['workHours'] });
    },
  });

  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editingWorkHour, setEditingWorkHour] = useState<BarberWorkHoursDto | null>(null);
  const [deletingWorkHour, setDeletingWorkHour] = useState<BarberWorkHoursDto | null>(null);
  const [formData, setFormData] = useState<CreateBarberWorkHoursDto>({
    barberId: 0,
    dayOfWeek: 1, // Monday by default
    startTime: '09:00',
    endTime: '17:00',
    isActive: true,
  });

  const handleOpenDialog = (workHour?: BarberWorkHoursDto) => {
    if (workHour) {
      setEditingWorkHour(workHour);
      setFormData({
        barberId: workHour.barberId,
        dayOfWeek: workHour.dayOfWeek,
        startTime: workHour.startTime,
        endTime: workHour.endTime,
        isActive: workHour.isActive,
      });
    } else {
      setEditingWorkHour(null);
      setFormData({
        barberId: barbers?.[0]?.barberId || 0,
        dayOfWeek: 1,
        startTime: '09:00',
        endTime: '17:00',
        isActive: true,
      });
    }
    setIsDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setIsDialogOpen(false);
    setEditingWorkHour(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      if (editingWorkHour) {
        await updateMutation.mutateAsync({ id: editingWorkHour.workHourId, data: formData });
      } else {
        await createMutation.mutateAsync(formData);
      }
      handleCloseDialog();
    } catch (error) {
      // Error handled by mutation
    }
  };

  const handleOpenDeleteDialog = (workHour: BarberWorkHoursDto) => {
    setDeletingWorkHour(workHour);
    setIsDeleteDialogOpen(true);
  };

  const handleCloseDeleteDialog = () => {
    setIsDeleteDialogOpen(false);
    setDeletingWorkHour(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingWorkHour) return;

    try {
      await deleteMutation.mutateAsync(deletingWorkHour.workHourId);
      handleCloseDeleteDialog();
    } catch (error) {
      // Error handled by mutation
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-muted-foreground">Loading work hours...</p>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Card>
        <CardContent className="py-12">
          <p className="text-center text-destructive">Error loading work hours</p>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Work Hours</CardTitle>
            <CardDescription>Manage barber availability and working hours</CardDescription>
          </div>
          <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
            <DialogTrigger asChild>
              <Button onClick={() => handleOpenDialog()}>
                <Plus className="h-4 w-4 mr-2" />
                Add Work Hours
              </Button>
            </DialogTrigger>
            <DialogContent className="max-w-md">
              <DialogHeader>
                <DialogTitle>{editingWorkHour ? 'Edit Work Hours' : 'Add New Work Hours'}</DialogTitle>
                <DialogDescription>
                  {editingWorkHour ? 'Update work hours for a barber' : 'Set working hours for a barber'}
                </DialogDescription>
              </DialogHeader>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-2">
                  <Label htmlFor="barberId">Barber</Label>
                  <Select
                    value={formData.barberId.toString()}
                    onValueChange={(value) => setFormData({ ...formData, barberId: parseInt(value) })}
                  >
                    <SelectTrigger id="barberId">
                      <SelectValue placeholder="Select a barber" />
                    </SelectTrigger>
                    <SelectContent>
                      {barbers?.map((barber) => (
                        <SelectItem key={barber.barberId} value={barber.barberId.toString()}>
                          {barber.name} ({barber.username})
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="dayOfWeek">Day of Week</Label>
                  <Select
                    value={formData.dayOfWeek.toString()}
                    onValueChange={(value) => setFormData({ ...formData, dayOfWeek: parseInt(value) })}
                  >
                    <SelectTrigger id="dayOfWeek">
                      <SelectValue placeholder="Select day" />
                    </SelectTrigger>
                    <SelectContent>
                      {daysOfWeek.map((day) => (
                        <SelectItem key={day.value} value={day.value.toString()}>
                          {day.label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="startTime">Start Time</Label>
                  <Input
                    id="startTime"
                    type="time"
                    required
                    value={formData.startTime}
                    onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="endTime">End Time</Label>
                  <Input
                    id="endTime"
                    type="time"
                    required
                    value={formData.endTime}
                    onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                  />
                </div>

                <div className="flex items-center space-x-2">
                  <Checkbox
                    id="isActive"
                    checked={formData.isActive}
                    onCheckedChange={(checked) => setFormData({ ...formData, isActive: checked as boolean })}
                  />
                  <label
                    htmlFor="isActive"
                    className="text-sm font-medium leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70 cursor-pointer"
                  >
                    Active (uncheck to mark as unavailable for holidays/vacation)
                  </label>
                </div>

                <Button type="submit" className="w-full" disabled={createMutation.isPending || updateMutation.isPending}>
                  {editingWorkHour
                    ? (updateMutation.isPending ? 'Updating...' : 'Update Work Hours')
                    : (createMutation.isPending ? 'Creating...' : 'Create Work Hours')
                  }
                </Button>
              </form>
            </DialogContent>
          </Dialog>
        </div>
      </CardHeader>
      <CardContent>
        {workHours && workHours.length === 0 ? (
          <p className="text-center py-8 text-muted-foreground">No work hours configured</p>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Barber</TableHead>
                <TableHead>Day</TableHead>
                <TableHead>Start Time</TableHead>
                <TableHead>End Time</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="text-right">Actions</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {workHours?.map((workHour) => (
                <TableRow key={workHour.workHourId}>
                  <TableCell>{workHour.workHourId}</TableCell>
                  <TableCell className="font-medium">{workHour.barberName}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{workHour.dayOfWeekName}</Badge>
                  </TableCell>
                  <TableCell>{workHour.startTime}</TableCell>
                  <TableCell>{workHour.endTime}</TableCell>
                  <TableCell>
                    <Badge variant={workHour.isActive ? 'default' : 'secondary'}>
                      {workHour.isActive ? 'Active' : 'Inactive'}
                    </Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    <div className="flex justify-end gap-2">
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenDialog(workHour)}
                      >
                        <Pencil className="h-4 w-4" />
                      </Button>
                      <Button
                        variant="ghost"
                        size="sm"
                        onClick={() => handleOpenDeleteDialog(workHour)}
                        disabled={deleteMutation.isPending}
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
      </CardContent>

      <Dialog open={isDeleteDialogOpen} onOpenChange={handleCloseDeleteDialog}>
        <DialogContent className="max-w-md">
          <DialogHeader>
            <DialogTitle>Delete Work Hours</DialogTitle>
            <DialogDescription>
              Are you sure you want to delete work hours for <strong>{deletingWorkHour?.barberName}</strong> on{' '}
              <strong>{deletingWorkHour?.dayOfWeekName}</strong>? This action cannot be undone.
            </DialogDescription>
          </DialogHeader>
          <div className="flex justify-end gap-3 mt-4">
            <Button variant="outline" onClick={handleCloseDeleteDialog}>
              Cancel
            </Button>
            <Button
              variant="destructive"
              onClick={handleConfirmDelete}
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? 'Deleting...' : 'Delete Work Hours'}
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </Card>
  );
}
