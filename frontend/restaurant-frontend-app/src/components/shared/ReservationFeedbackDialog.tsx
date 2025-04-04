import { ReactElement, useState } from "react";
import {
  Button,
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
  Text,
} from "../ui";
import { Star } from "../icons";
import { StarRating, Textarea } from ".";
import { useReservationFeedbackDialog } from "@/hooks/useReservationFeedbackDialog";

export const ReservationFeedbackDialog = (props: Props) => {
  const [isCurrentDialogOpen, setIsCurrentDialogOpen] = useState(false);
  const onSuccessCallback = () => {
    setIsCurrentDialogOpen(false);
  };
  const state = useReservationFeedbackDialog({
    reservationId: props.reservationId,
    onSuccessCallback,
  });

  return (
    <Dialog open={isCurrentDialogOpen} onOpenChange={setIsCurrentDialogOpen}>
      <DialogTrigger asChild>{props.children}</DialogTrigger>
      <DialogContent className="bg-neutral-100">
        <DialogHeader>
          <DialogTitle className="!fontset-h2">Give Feedback</DialogTitle>
          <DialogDescription className="!fontset-body text-foreground">
            Please rate your experience below
          </DialogDescription>
        </DialogHeader>
        <div>
          <Tabs defaultValue="service">
            <TabsList className="grid grid-cols-2">
              <TabsTrigger value="service" className="py-[0.5rem] ">
                Service
              </TabsTrigger>
              <TabsTrigger value="culinary" className="py-[0.5rem] ">
                Culinary Experience
              </TabsTrigger>
            </TabsList>
            <TabsContent value="service">
              <div className="grid grid-cols-[88px_1fr] gap-[1rem] mt-[2.5rem]">
                <img
                  src="/cook-thumbnail.jpg"
                  className="size-full object-cover rounded-full aspect-square"
                  alt="Mario Jast"
                />
                <div>
                  <Text variant="bodyBold">Mario Jast</Text>
                  <Text variant="body">Waiter</Text>
                  <Text
                    variant="body"
                    className="flex items-center gap-[0.25rem] mt-[0.5rem]"
                  >
                    4.96{" "}
                    <Star className="size-[1rem] fill-orange-400 stroke-orange-400" />
                  </Text>
                </div>
              </div>
              <div className="flex justify-between items-center mt-[2.5rem]">
                <StarRating
                  selected={state.serviceRating}
                  onChange={state.setServiceRating}
                />
                <Text variant="bodyBold">{state.serviceRating} / 5 stars</Text>
              </div>
              <Textarea
                className="mt-[2.5rem]"
                rows={3}
                placeholder="Add your comments"
                value={state.serviceComment}
                onChange={(e) => state.setServiceComment(e.target.value)}
              />
            </TabsContent>
            <TabsContent value="culinary">
              <div className="flex justify-between items-center mt-[2.5rem]">
                <StarRating
                  selected={state.culinaryRating}
                  onChange={state.setCulinaryRating}
                />
                <Text variant="bodyBold">{state.culinaryRating} / 5 stars</Text>
              </div>
              <Textarea
                className="mt-[2.5rem]"
                rows={3}
                placeholder="Add your comments"
                value={state.culinaryComment}
                onChange={(e) => state.setCulinaryComment(e.target.value)}
              />
            </TabsContent>
          </Tabs>
        </div>
        <DialogFooter>
          <Button type="submit" className="w-full">
            Submit Feedback
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

interface Props {
  children: ReactElement;
  reservationId: string;
}
