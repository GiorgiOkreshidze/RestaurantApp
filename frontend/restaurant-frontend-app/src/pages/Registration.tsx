import { Container } from "@/components/shared";
import { Button, Input, Label } from "@/components/ui";
import React from "react";

const Registration = () => {
  return (
    <Container>
      <div className="flex w-full">
        <div className="w-1/2 px-[84px] py-[34px]">
          <h3 className="uppercase text-sm block">LET'S GET YOU STARTED</h3>
          <h2 className="text-2xl block mb-[64px]">Create an Account</h2>
          <form>
            <div className="flex gap-4">
              <div className="w-full mb-6">
                <Label
                  htmlFor="name"
                  className="font-medium mb-1 leading-[24px]"
                >
                  First Name
                </Label>
                <Input
                  id="name"
                  name="name"
                  placeholder="Enter your First Name"
                  className="h-[56px] w-full bg-primary"
                />
                <Label
                  htmlFor="name"
                  className="text-xs font-light text-neutral"
                >
                  e.g. Jonson
                </Label>
              </div>

              <div className="w-full">
                <Label
                  htmlFor="name"
                  className="font-medium mb-1 leading-[24px]"
                >
                  Last Name
                </Label>
                <Input
                  id="lastName"
                  name="lastName"
                  placeholder="Enter your Last Name"
                  className="h-[56px] w-full bg-primary"
                />
                <Label
                  htmlFor="lastName"
                  className="text-xs font-light text-neutral"
                >
                  e.g. Doe
                </Label>
              </div>
            </div>

            <div>
              <div className="w-full mb-6">
                <Label
                  htmlFor="name"
                  className="font-medium mb-1 leading-[24px]"
                >
                  Email
                </Label>
                <Input
                  id="email"
                  name="email"
                  placeholder="Enter your Email"
                  className="h-[56px] w-full bg-primary"
                />
                <Label
                  htmlFor="email"
                  className="text-xs font-light text-neutral"
                >
                  e.g. username@domain.com
                </Label>
              </div>
            </div>

            <div>
              <div className="w-full mb-6">
                <Label
                  htmlFor="password"
                  className="font-medium mb-1 leading-[24px]"
                >
                  Password
                </Label>
                <Input
                  id="password"
                  name="password"
                  placeholder="Enter your Password"
                  className="h-[56px] w-full bg-primary"
                />
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full mt-1"
                >
                  At least one uppercase letter required
                </Label>
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full "
                >
                  At least one lowercase letter required
                </Label>
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full "
                >
                  At least one number required
                </Label>
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full "
                >
                  At least one character required
                </Label>
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full"
                >
                  Password must be 8-16 characters long
                </Label>
              </div>
            </div>

            <div>
              <div className="w-full mb-[64px]">
                <Label
                  htmlFor="confirm"
                  className="font-medium mb-1 leading-[24px]"
                >
                  Confirm New Password
                </Label>
                <Input
                  id="confirm"
                  name="confirm"
                  placeholder="Confirm New Password"
                  className="h-[56px] w-full bg-primary"
                />
                <Label
                  htmlFor="password"
                  className="text-xs font-light text-neutral before:content-[''] before:block before:w-[8px] before:h-[8px] before:bg-neutral before:rounded-full "
                >
                  Confirm password must match new password
                </Label>
              </div>
            </div>

            <div>
              <Button className="bg-secondary w-full h-[56px] rounded-[8px] border">
                Create an Account
              </Button>
              <p className="font-light text-xs mt-4">Already have an account? <a href="/login" className="text-blue font-bold ">Login </a>instead</p>
            </div>
          </form>
        </div>

        <div className="w-1/2 p-4">
          <div className="w-full">
            <img src="src/assets/images/registrationImage.svg" alt="" />
          </div>
        </div>
      </div>
    </Container>
  );
};

export default Registration;
